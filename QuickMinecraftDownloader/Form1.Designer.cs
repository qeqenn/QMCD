namespace QuickMinecraftDownloader
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            menuStrip1 = new MenuStrip();
            文件ToolStripMenuItem = new ToolStripMenuItem();
            退出ToolStripMenuItem = new ToolStripMenuItem();
            选项ToolStripMenuItem = new ToolStripMenuItem();
            清除ToolStripMenuItem = new ToolStripMenuItem();
            历史记录ToolStripMenuItem = new ToolStripMenuItem();
            缓存ToolStripMenuItem = new ToolStripMenuItem();
            安装ToolStripMenuItem = new ToolStripMenuItem();
            java运行时ToolStripMenuItem = new ToolStripMenuItem();
            整合包ToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            输出文件夹 = new ComboBox();
            选择文件夹 = new Button();
            label2 = new Label();
            GameVers = new ComboBox();
            正式版 = new RadioButton();
            GameVerPanel = new Panel();
            远古版 = new RadioButton();
            愚人节版 = new RadioButton();
            预览版 = new RadioButton();
            刷新GameVer = new Button();
            label3 = new Label();
            ModLoaderPanel = new Panel();
            Optifine = new RadioButton();
            Fabric = new RadioButton();
            NeoForge = new RadioButton();
            Forge = new RadioButton();
            无加载器 = new RadioButton();
            扫描文件夹 = new Button();
            开始安装 = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            安装进度 = new ProgressBar();
            加载器版本 = new ComboBox();
            menuStrip1.SuspendLayout();
            GameVerPanel.SuspendLayout();
            ModLoaderPanel.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Control;
            menuStrip1.Items.AddRange(new ToolStripItem[] { 文件ToolStripMenuItem, 选项ToolStripMenuItem, 安装ToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            文件ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 退出ToolStripMenuItem });
            文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            文件ToolStripMenuItem.Size = new Size(44, 21);
            文件ToolStripMenuItem.Text = "文件";
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new Size(100, 22);
            退出ToolStripMenuItem.Text = "退出";
            退出ToolStripMenuItem.Click += 退出ToolStripMenuItem_Click;
            // 
            // 选项ToolStripMenuItem
            // 
            选项ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 清除ToolStripMenuItem });
            选项ToolStripMenuItem.Name = "选项ToolStripMenuItem";
            选项ToolStripMenuItem.Size = new Size(44, 21);
            选项ToolStripMenuItem.Text = "选项";
            // 
            // 清除ToolStripMenuItem
            // 
            清除ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 历史记录ToolStripMenuItem, 缓存ToolStripMenuItem });
            清除ToolStripMenuItem.Name = "清除ToolStripMenuItem";
            清除ToolStripMenuItem.Size = new Size(100, 22);
            清除ToolStripMenuItem.Text = "清除";
            // 
            // 历史记录ToolStripMenuItem
            // 
            历史记录ToolStripMenuItem.Name = "历史记录ToolStripMenuItem";
            历史记录ToolStripMenuItem.Size = new Size(124, 22);
            历史记录ToolStripMenuItem.Text = "历史记录";
            历史记录ToolStripMenuItem.Click += 历史记录ToolStripMenuItem_Click;
            // 
            // 缓存ToolStripMenuItem
            // 
            缓存ToolStripMenuItem.Name = "缓存ToolStripMenuItem";
            缓存ToolStripMenuItem.Size = new Size(124, 22);
            缓存ToolStripMenuItem.Text = "缓存";
            缓存ToolStripMenuItem.Click += 缓存ToolStripMenuItem_Click;
            // 
            // 安装ToolStripMenuItem
            // 
            安装ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { java运行时ToolStripMenuItem, 整合包ToolStripMenuItem });
            安装ToolStripMenuItem.Name = "安装ToolStripMenuItem";
            安装ToolStripMenuItem.Size = new Size(44, 21);
            安装ToolStripMenuItem.Text = "安装";
            // 
            // java运行时ToolStripMenuItem
            // 
            java运行时ToolStripMenuItem.Enabled = false;
            java运行时ToolStripMenuItem.Name = "java运行时ToolStripMenuItem";
            java运行时ToolStripMenuItem.Size = new Size(180, 22);
            java运行时ToolStripMenuItem.Text = "Java运行时";
            // 
            // 整合包ToolStripMenuItem
            // 
            整合包ToolStripMenuItem.Enabled = false;
            整合包ToolStripMenuItem.Name = "整合包ToolStripMenuItem";
            整合包ToolStripMenuItem.Size = new Size(180, 22);
            整合包ToolStripMenuItem.Text = "整合包";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 31);
            label1.Name = "label1";
            label1.Size = new Size(80, 17);
            label1.TabIndex = 1;
            label1.Text = "输出文件夹：";
            // 
            // 输出文件夹
            // 
            输出文件夹.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            输出文件夹.FormattingEnabled = true;
            输出文件夹.Location = new Point(98, 28);
            输出文件夹.Name = "输出文件夹";
            输出文件夹.Size = new Size(526, 25);
            输出文件夹.TabIndex = 2;
            // 
            // 选择文件夹
            // 
            选择文件夹.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            选择文件夹.Location = new Point(630, 30);
            选择文件夹.Name = "选择文件夹";
            选择文件夹.Size = new Size(83, 23);
            选择文件夹.TabIndex = 3;
            选择文件夹.Text = "选择文件夹";
            选择文件夹.UseVisualStyleBackColor = true;
            选择文件夹.Click += 选择文件夹_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 66);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 4;
            label2.Text = "选择版本：";
            // 
            // GameVers
            // 
            GameVers.DropDownStyle = ComboBoxStyle.DropDownList;
            GameVers.FormattingEnabled = true;
            GameVers.Location = new Point(98, 63);
            GameVers.Name = "GameVers";
            GameVers.Size = new Size(156, 25);
            GameVers.TabIndex = 5;
            GameVers.SelectedIndexChanged += GameVers_SelectedIndexChanged;
            // 
            // 正式版
            // 
            正式版.AutoSize = true;
            正式版.Location = new Point(3, 3);
            正式版.Name = "正式版";
            正式版.Size = new Size(62, 21);
            正式版.TabIndex = 7;
            正式版.TabStop = true;
            正式版.Text = "正式版";
            正式版.UseVisualStyleBackColor = true;
            正式版.CheckedChanged += 正式版_CheckedChanged;
            // 
            // GameVerPanel
            // 
            GameVerPanel.Controls.Add(远古版);
            GameVerPanel.Controls.Add(愚人节版);
            GameVerPanel.Controls.Add(预览版);
            GameVerPanel.Controls.Add(正式版);
            GameVerPanel.Location = new Point(312, 63);
            GameVerPanel.Name = "GameVerPanel";
            GameVerPanel.Size = new Size(293, 25);
            GameVerPanel.TabIndex = 8;
            // 
            // 远古版
            // 
            远古版.AutoSize = true;
            远古版.Location = new Point(219, 3);
            远古版.Name = "远古版";
            远古版.Size = new Size(62, 21);
            远古版.TabIndex = 10;
            远古版.TabStop = true;
            远古版.Text = "远古版";
            远古版.UseVisualStyleBackColor = true;
            远古版.CheckedChanged += 远古版_CheckedChanged;
            // 
            // 愚人节版
            // 
            愚人节版.AutoSize = true;
            愚人节版.Location = new Point(139, 3);
            愚人节版.Name = "愚人节版";
            愚人节版.Size = new Size(74, 21);
            愚人节版.TabIndex = 9;
            愚人节版.TabStop = true;
            愚人节版.Text = "愚人节版";
            愚人节版.UseVisualStyleBackColor = true;
            愚人节版.CheckedChanged += 愚人节版_CheckedChanged;
            // 
            // 预览版
            // 
            预览版.AutoSize = true;
            预览版.Location = new Point(71, 3);
            预览版.Name = "预览版";
            预览版.Size = new Size(62, 21);
            预览版.TabIndex = 8;
            预览版.TabStop = true;
            预览版.Text = "预览版";
            预览版.UseVisualStyleBackColor = true;
            预览版.CheckedChanged += 预览版_CheckedChanged;
            // 
            // 刷新GameVer
            // 
            刷新GameVer.Location = new Point(260, 64);
            刷新GameVer.Name = "刷新GameVer";
            刷新GameVer.Size = new Size(46, 23);
            刷新GameVer.TabIndex = 9;
            刷新GameVer.Text = "刷新";
            刷新GameVer.UseVisualStyleBackColor = true;
            刷新GameVer.Click += 刷新GameVer_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 104);
            label3.Name = "label3";
            label3.Size = new Size(80, 17);
            label3.TabIndex = 10;
            label3.Text = "选择加载器：";
            // 
            // ModLoaderPanel
            // 
            ModLoaderPanel.Controls.Add(Optifine);
            ModLoaderPanel.Controls.Add(Fabric);
            ModLoaderPanel.Controls.Add(NeoForge);
            ModLoaderPanel.Controls.Add(Forge);
            ModLoaderPanel.Controls.Add(无加载器);
            ModLoaderPanel.Location = new Point(98, 100);
            ModLoaderPanel.Name = "ModLoaderPanel";
            ModLoaderPanel.Size = new Size(358, 25);
            ModLoaderPanel.TabIndex = 11;
            // 
            // Optifine
            // 
            Optifine.AutoSize = true;
            Optifine.Enabled = false;
            Optifine.Location = new Point(271, 3);
            Optifine.Name = "Optifine";
            Optifine.Size = new Size(72, 21);
            Optifine.TabIndex = 14;
            Optifine.TabStop = true;
            Optifine.Text = "Optifine";
            Optifine.UseVisualStyleBackColor = true;
            Optifine.CheckedChanged += Optifine_CheckedChanged;
            // 
            // Fabric
            // 
            Fabric.AutoSize = true;
            Fabric.Enabled = false;
            Fabric.Location = new Point(204, 3);
            Fabric.Name = "Fabric";
            Fabric.Size = new Size(61, 21);
            Fabric.TabIndex = 13;
            Fabric.Text = "Fabric";
            Fabric.UseVisualStyleBackColor = true;
            Fabric.CheckedChanged += Fabric_CheckedChanged;
            // 
            // NeoForge
            // 
            NeoForge.AutoSize = true;
            NeoForge.Enabled = false;
            NeoForge.Location = new Point(113, 3);
            NeoForge.Name = "NeoForge";
            NeoForge.Size = new Size(85, 21);
            NeoForge.TabIndex = 12;
            NeoForge.TabStop = true;
            NeoForge.Text = "NeoForge";
            NeoForge.UseVisualStyleBackColor = true;
            NeoForge.CheckedChanged += NeoForge_CheckedChanged;
            // 
            // Forge
            // 
            Forge.AutoSize = true;
            Forge.Enabled = false;
            Forge.Location = new Point(47, 3);
            Forge.Name = "Forge";
            Forge.Size = new Size(60, 21);
            Forge.TabIndex = 1;
            Forge.TabStop = true;
            Forge.Text = "Forge";
            Forge.UseVisualStyleBackColor = true;
            Forge.CheckedChanged += Forge_CheckedChanged;
            // 
            // 无加载器
            // 
            无加载器.AutoSize = true;
            无加载器.Location = new Point(3, 3);
            无加载器.Name = "无加载器";
            无加载器.Size = new Size(38, 21);
            无加载器.TabIndex = 0;
            无加载器.TabStop = true;
            无加载器.Text = "无";
            无加载器.UseVisualStyleBackColor = true;
            无加载器.CheckedChanged += 无加载器_CheckedChanged;
            // 
            // 扫描文件夹
            // 
            扫描文件夹.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            扫描文件夹.Location = new Point(719, 30);
            扫描文件夹.Name = "扫描文件夹";
            扫描文件夹.Size = new Size(75, 23);
            扫描文件夹.TabIndex = 12;
            扫描文件夹.Text = "扫描";
            扫描文件夹.UseVisualStyleBackColor = true;
            扫描文件夹.Click += 扫描文件夹_Click;
            // 
            // 开始安装
            // 
            开始安装.Location = new Point(12, 138);
            开始安装.Name = "开始安装";
            开始安装.Size = new Size(75, 23);
            开始安装.TabIndex = 13;
            开始安装.Text = "开始安装";
            开始安装.UseVisualStyleBackColor = true;
            开始安装.Click += 开始安装_Click;
            // 
            // 安装进度
            // 
            安装进度.Dock = DockStyle.Bottom;
            安装进度.Location = new Point(0, 427);
            安装进度.Name = "安装进度";
            安装进度.Size = new Size(800, 23);
            安装进度.TabIndex = 14;
            // 
            // 加载器版本
            // 
            加载器版本.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            加载器版本.DropDownStyle = ComboBoxStyle.DropDownList;
            加载器版本.FormattingEnabled = true;
            加载器版本.Location = new Point(462, 100);
            加载器版本.Name = "加载器版本";
            加载器版本.Size = new Size(162, 25);
            加载器版本.TabIndex = 15;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(加载器版本);
            Controls.Add(安装进度);
            Controls.Add(开始安装);
            Controls.Add(扫描文件夹);
            Controls.Add(ModLoaderPanel);
            Controls.Add(label3);
            Controls.Add(刷新GameVer);
            Controls.Add(GameVerPanel);
            Controls.Add(GameVers);
            Controls.Add(label2);
            Controls.Add(选择文件夹);
            Controls.Add(输出文件夹);
            Controls.Add(label1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "QMCD";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            GameVerPanel.ResumeLayout(false);
            GameVerPanel.PerformLayout();
            ModLoaderPanel.ResumeLayout(false);
            ModLoaderPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem 文件ToolStripMenuItem;
        private Label label1;
        private ComboBox 输出文件夹;
        private Button 选择文件夹;
        private Label label2;
        private ComboBox GameVers;
        private RadioButton 正式版;
        private Panel GameVerPanel;
        private RadioButton 愚人节版;
        private RadioButton 预览版;
        private Button 刷新GameVer;
        private Label label3;
        private Panel ModLoaderPanel;
        private RadioButton Fabric;
        private RadioButton NeoForge;
        private RadioButton Forge;
        private RadioButton 无加载器;
        private RadioButton Optifine;
        private ToolStripMenuItem 退出ToolStripMenuItem;
        private Button 扫描文件夹;
        private Button 开始安装;
        private FolderBrowserDialog folderBrowserDialog1;
        private RadioButton 远古版;
        private ProgressBar 安装进度;
        private ToolStripMenuItem 选项ToolStripMenuItem;
        private ToolStripMenuItem 清除ToolStripMenuItem;
        private ToolStripMenuItem 历史记录ToolStripMenuItem;
        private ToolStripMenuItem 缓存ToolStripMenuItem;
        private ComboBox 加载器版本;
        private ToolStripMenuItem 安装ToolStripMenuItem;
        private ToolStripMenuItem java运行时ToolStripMenuItem;
        private ToolStripMenuItem 整合包ToolStripMenuItem;
    }
}
