using QuickMinecraftDownloader.Models;
using QuickMinecraftDownloader.Services;
using System.Text.Json;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QuickMinecraftDownloader
{
    public partial class Form1 : Form
    {
        private JsonRepository<MemoryItem> _db;
        private List<VersionEntry> _cachedVersions;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "QuickMinecraftDownloader", "memories.json");
            _db = new JsonRepository<MemoryItem>(dbPath, x => x.Id);

            //查询上一次的文件夹
            var history = _db.Query(x => x.Name == "LastFolder")
                     .OrderByDescending(x => x.CreatedAt)
                     .ToList();

            // 去重：同一个路径只保留最近一条
            history = history
                .GroupBy(x => x.Value)
                .Select(g => g.First())
                .ToList();

            // 只把存在的文件夹放进下拉框
            var paths = history
                .Where(x => Directory.Exists(x.Value))
                .Select(x => x.Value)
                .ToList();

            输出文件夹.DataSource = null;          // 先解绑
            输出文件夹.DataSource = paths;

            // 默认选中最近的一条
            if (paths.Count > 0)
                输出文件夹.SelectedIndex = 0;

            // 刷新版本列表
            刷新GameVer_Click(null, null);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 选择文件夹_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            // 2. 设置对话框属性
            dialog.Description = "选择输出文件夹"; // 提示文字
            dialog.RootFolder = Environment.SpecialFolder.MyComputer; // 设置起始根目录
            dialog.SelectedPath = @".minecraft"; // 设置默认选中的路径
            dialog.ShowNewFolderButton = true; // 是否显示"新建文件夹"按钮（默认为true）

            // 3. 显示对话框并判断用户操作
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // 4. 获取用户选择的文件夹路径
                string selectedPath = dialog.SelectedPath;

                // 将路径显示在文本框
                输出文件夹.Text = selectedPath;
            }
            //输出至JSON
            var item = new MemoryItem
            {
                Name = "LastFolder",
                Value = dialog.SelectedPath
            };
            _db.Add(item);

        }

        private async void 扫描文件夹_Click(object sender, EventArgs e)
        {
            //扫描.minecraft

            string keyword = ".minecraft";

            var drives = DriveScanner.GetAllDrives();
            if (drives.Count == 0)
            {
                MessageBox.Show("没有可扫描的磁盘。", "警告");
                return;
            }

            扫描文件夹.Enabled = 开始安装.Enabled = false;
            Cursor = Cursors.WaitCursor;

            // 用 Items 模式：全程不设 DataSource
            输出文件夹.DataSource = null;
            输出文件夹.Items.Clear();
            输出文件夹.Items.Add($"扫描中：{string.Join(", ", drives)}");
            输出文件夹.SelectedIndex = 0;

            try
            {
                // 每个盘一个后台任务，并行跑
                var tasks = drives.Select(drive => Task.Run(() =>
                    FolderScanner.Scan(drive, keyword, maxDepth: 6, maxResults: 200)))
                    .ToArray();

                var resultsPerDrive = await Task.WhenAll(tasks);

                // 合并 → 去重 → 排序
                var allPaths = resultsPerDrive
                    .SelectMany(x => x)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // 刷新 ComboBox
                输出文件夹.Items.Clear();
                if (allPaths.Count == 0)
                {
                    MessageBox.Show("没有找到MC游戏文件夹。", "提示");
                }
                else
                {
                    foreach (var p in allPaths)
                        输出文件夹.Items.Add(p);

                    输出文件夹.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("扫描出错：" + ex.Message, "错误");
            }
            finally
            {
                输出文件夹.DropDownStyle = ComboBoxStyle.DropDownList;
                扫描文件夹.Enabled = 开始安装.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async void 刷新GameVer_Click(object sender, EventArgs e)
        {
            刷新GameVer.Enabled = 开始安装.Enabled = false;
            Cursor = Cursors.WaitCursor;

            GameVers.DataSource = null;
            GameVers.Items.Clear();
            GameVers.Items.Add("正在获取版本列表...");

            try
            {
                var manifest = await VersionManifestService.GetAsync();
                _cachedVersions = manifest.Versions;
                ApplyVersionFilter();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"网络请求失败：{ex.Message}", "错误");
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"JSON 解析失败：{ex.Message}", "错误");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新出错：{ex.Message}", "错误");
            }
            finally
            {
                GameVers.DropDownStyle = ComboBoxStyle.DropDownList;
                刷新GameVer.Enabled = 开始安装.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
        private void ApplyVersionFilter()
        {
            if (_cachedVersions == null) return;

            IEnumerable<VersionEntry> filtered;

            if (正式版.Checked)
            {
                filtered = _cachedVersions.Where(v => v.Type == "release");
            }
            else if (预览版.Checked)
            {
                filtered = _cachedVersions.Where(v => v.Type == "snapshot" && !IsAprilFools(v));
            }
            else if (愚人节版.Checked)
            {
                filtered = _cachedVersions.Where(v => IsAprilFools(v));
            }
            else if (远古版.Checked)
            {
                filtered = _cachedVersions.Where(v => v.Type == "old_alpha" || v.Type == "old_beta");
            }
            else
            {
                filtered = _cachedVersions.Where(v => v.Type == "release");
            }

            var ids = filtered.Select(v => v.Id).ToList();

            GameVers.DataSource = null;
            GameVers.Items.Clear();

            if (ids.Count == 0)
            {
                GameVers.Items.Add("未找到匹配版本");
                return;
            }

            foreach (var id in ids)
                GameVers.Items.Add(id);

            GameVers.SelectedIndex = 0;
        }
        private static bool IsAprilFools(VersionEntry v)
        {
            if (string.IsNullOrEmpty(v.ReleaseTime)) return false;
            if (!DateTimeOffset.TryParse(v.ReleaseTime, out var dto)) return false;

            var utc = dto.UtcDateTime;
            return utc.Month == 4 && utc.Day == 1;
        }

        private void 正式版_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;
            ApplyVersionFilter();
        }

        private void 预览版_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;
            ApplyVersionFilter();
        }

        private void 愚人节版_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;
            ApplyVersionFilter();
        }

        private void 远古版_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;
            ApplyVersionFilter();
        }

        /// <summary>判断路径最后一级目录名是否为 .minecraft（忽略大小写）</summary>
        private static bool IsMinecraftFolder(string path)
        {
            string trimmed = path.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

            string name = Path.GetFileName(trimmed);
            return string.Equals(name, ".minecraft", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 返回真正要写入的 .minecraft 路径：
        /// 选中目录本身就是 .minecraft → 直接用；
        /// 否则在其下创建 .minecraft。
        /// </summary>
        private static string EnsureMinecraftDir(string selectedFolder)
        {
            if (!Directory.Exists(selectedFolder))
                Directory.CreateDirectory(selectedFolder);

            if (IsMinecraftFolder(selectedFolder))
            {
                return selectedFolder.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar);
            }

            string mcDir = Path.Combine(selectedFolder, ".minecraft");
            Directory.CreateDirectory(mcDir);
            return mcDir;
        }

        private async void 开始安装_Click(object sender, EventArgs e)
        {
            // ---- 校验（同上）----
            if (GameVers.SelectedItem == null ||
                GameVers.SelectedItem.ToString() == "未找到匹配版本")
            {
                MessageBox.Show("请先选择游戏版本。", "提示");
                return;
            }
            string versionId = GameVers.SelectedItem.ToString();

            string selectedFolder = 输出文件夹.Text?.Trim();
            if (string.IsNullOrEmpty(selectedFolder))
            {
                MessageBox.Show("请先选择输出文件夹。", "提示");
                return;
            }

            var entry = _cachedVersions?.FirstOrDefault(v => v.Id == versionId);
            if (entry == null)
            {
                MessageBox.Show("未找到版本信息，请先点击「刷新游戏版本」。", "提示");
                return;
            }

            // ---- 准备 UI ----
            开始安装.Enabled = false;
            Cursor = Cursors.WaitCursor;
            安装进度.Value = 0;

            try
            {
                string mcDir = EnsureMinecraftDir(selectedFolder);

                await InstallVersionAsync(entry, mcDir, (percent, msg) =>
                {
                    // 后台线程 → 必须 Invoke 回 UI 线程
                    this.Invoke(new Action(() =>
                    {
                        // 夹在 0~100，避免异常数据把 Value 设越界
                        int v = Math.Max(0, Math.Min(100, percent));
                        安装进度.Value = v;

                        // 可选：同步文字到 Label
                        // lblStatus.Text = msg;
                    }));
                });

                安装进度.Value = 100;
                MessageBox.Show($"版本 {versionId} 安装完成！\n位置：{mcDir}", "完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show("安装失败：" + ex.Message, "错误");
            }
            finally
            {
                开始安装.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        //安装逻辑
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10)
        };

        /// <summary>
        /// 下载指定版本到 .minecraft 目录。
        /// </summary>
        /// <param name="entry">版本清单里的条目（含 version json 的 URL）</param>
        /// <param name="mcDir">规范化后的 .minecraft 路径</param>
        /// <param name="report">进度回调（在后台线程调用）</param>
        /// <param name="report">进度回调 (百分比, 文字)，在后台线程调用</param>
        private async Task InstallVersionAsync(
            VersionEntry entry, string mcDir, Action<int, string> report)
        {
            string versionDir = Path.Combine(mcDir, "versions", entry.Id);
            Directory.CreateDirectory(versionDir);

            // ---- 阶段 1：version json（占 0~10%）----
            report(0, $"下载 {entry.Id}.json ...");
            string jsonPath = Path.Combine(versionDir, entry.Id + ".json");
            string versionJson = await _http.GetStringAsync(entry.Url);
            File.WriteAllText(jsonPath, versionJson);
            report(10, $"{entry.Id}.json 下载完成");

            // ---- 解析 client jar 地址 ----
            string clientUrl;
            using (var doc = JsonDocument.Parse(versionJson))
            {
                if (!doc.RootElement.TryGetProperty("downloads", out var downloads) ||
                    !downloads.TryGetProperty("client", out var client) ||
                    !client.TryGetProperty("url", out var urlEl))
                {
                    throw new InvalidOperationException("版本 JSON 中缺少 client 下载地址。");
                }
                clientUrl = urlEl.GetString();
            }

            // ---- 阶段 2：client jar（占 10~100%）----
            report(10, $"下载 {entry.Id}.jar ...");
            string jarPath = Path.Combine(versionDir, entry.Id + ".jar");

            using (var response = await _http.GetAsync(
                clientUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                long? total = response.Content.Headers.ContentLength;

                using var fs = new FileStream(
                    jarPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);
                using var stream = await response.Content.ReadAsStreamAsync();

                var buffer = new byte[81920];
                long readTotal = 0;
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fs.WriteAsync(buffer, 0, read);
                    readTotal += read;

                    if (total.HasValue && total.Value > 0)
                    {
                        // 映射到 10~100
                        int percent = 10 + (int)(readTotal * 90 / total.Value);
                        report(percent, $"{entry.Id}.jar 下载中 {percent}%");
                    }
                }
            }

            report(100, $"{entry.Id} 下载完成");
        }
    }
}
