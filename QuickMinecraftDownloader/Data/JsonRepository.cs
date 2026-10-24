using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class JsonRepository<T>
{
    private readonly string _filePath;
    private readonly Func<T, string> _keySelector;
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 中文不转义
    };

    private List<T> _items = new List<T>();

    /// <summary>
    /// </summary>
    /// <param name="filePath">JSON 文件完整路径</param>
    /// <param name="keySelector">如何取主键，例如 x => x.Id</param>
    public JsonRepository(string filePath, Func<T, string> keySelector)
    {
        _filePath = filePath;
        _keySelector = keySelector;
        Load();
    }

    // ========== 加载 / 保存 ==========

    private void Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _items = JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
            }
        }
        catch
        {
            _items = new List<T>(); // 文件损坏时从空库开始，不崩溃
        }
    }

    private void Save()
    {
        string dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json = JsonSerializer.Serialize(_items, _options);

        // 原子写入：先写临时文件再替换，避免写一半崩溃导致文件损坏
        string tmp = _filePath + ".tmp";
        File.WriteAllText(tmp, json);
        File.Copy(tmp, _filePath, true);
        File.Delete(tmp);
    }

    // ========== 创建 ==========

    /// <summary>新增一条，主键重复则抛异常</summary>
    public void Add(T item)
    {
        string key = _keySelector(item);
        if (_items.Any(x => _keySelector(x) == key))
            throw new InvalidOperationException($"主键已存在：{key}");

        _items.Add(item);
        Save();
    }

    /// <summary>存在则更新，不存在则新增（Upsert）</summary>
    public void Save(T item)
    {
        string key = _keySelector(item);
        int index = _items.FindIndex(x => _keySelector(x) == key);

        if (index >= 0) _items[index] = item;
        else _items.Add(item);

        Save();
    }

    // ========== 查询 ==========

    /// <summary>按主键查一条，找不到返回 default</summary>
    public T GetById(string id)
    {
        return _items.FirstOrDefault(x => _keySelector(x) == id);
    }

    /// <summary>取全部</summary>
    public List<T> GetAll()
    {
        return _items.ToList(); // 返回副本，防止外部误改
    }

    /// <summary>按条件查询</summary>
    public List<T> Query(Func<T, bool> predicate)
    {
        return _items.Where(predicate).ToList();
    }

    /// <summary>是否存在</summary>
    public bool Exists(string id)
    {
        return _items.Any(x => _keySelector(x) == id);
    }

    /// <summary>总数</summary>
    public int Count => _items.Count;

    // ========== 删除 ==========

    /// <summary>按主键删除，返回是否删掉</summary>
    public bool Delete(string id)
    {
        int removed = _items.RemoveAll(x => _keySelector(x) == id);
        if (removed > 0) Save();
        return removed > 0;
    }

    /// <summary>按条件批量删除，返回删除条数</summary>
    public int DeleteWhere(Func<T, bool> predicate)
    {
        int removed = _items.RemoveAll(x => predicate(x));
        if (removed > 0) Save();
        return removed;
    }

    /// <summary>清空所有记录（保留库文件）</summary>
    public void Clear()
    {
        _items.Clear();
        Save();
    }

    /// <summary>删除整个库文件</summary>
    public void DropDatabase()
    {
        _items.Clear();
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }
}