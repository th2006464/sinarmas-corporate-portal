using System.Text.Json;
using System.Text.Json.Serialization;

namespace Portal.Services;

public class NewsItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("date")]
    public string Date { get; set; } = "";

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = "";

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";

    [JsonPropertyName("video_url")]
    public string VideoUrl { get; set; } = "";
}

public class NewsData
{
    [JsonPropertyName("news")]
    public List<NewsItem> News { get; set; } = new();
}

public class NewsService
{
    private readonly string _dataPath;
    private List<NewsItem>? _cache;

    public NewsService(IWebHostEnvironment env)
    {
        _dataPath = Path.Combine(env.ContentRootPath, "data", "news.json");
    }

    private List<NewsItem> Load()
    {
        if (_cache != null) return _cache;
        if (!File.Exists(_dataPath)) return _cache = new List<NewsItem>();

        var json = File.ReadAllText(_dataPath);
        var data = JsonSerializer.Deserialize<NewsData>(json);
        return _cache = data?.News ?? new List<NewsItem>();
    }

    private void Save()
    {
        var dir = Path.GetDirectoryName(_dataPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var data = new NewsData { News = _cache ?? new List<NewsItem>() };
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_dataPath, json);
    }

    public List<NewsItem> GetAll()
    {
        return Load().OrderByDescending(n => n.Date).ToList();
    }

    public List<NewsItem> GetLatest(int count)
    {
        return Load().OrderByDescending(n => n.Date).Take(count).ToList();
    }

    public NewsItem? GetById(int id)
    {
        return Load().FirstOrDefault(n => n.Id == id);
    }

    public (List<NewsItem> Items, int TotalPages) GetPaged(int page, int perPage)
    {
        var all = Load().OrderByDescending(n => n.Date).ToList();
        int total = (int)Math.Ceiling((double)all.Count / perPage);
        var items = all.Skip((page - 1) * perPage).Take(perPage).ToList();
        return (items, total);
    }

    public List<NewsItem> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<NewsItem>();
        return Load()
            .Where(n => n.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || n.Summary.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || n.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(n => n.Date)
            .ToList();
    }

    // === Admin CRUD ===

    public NewsItem Add(NewsItem item)
    {
        var list = Load();
        item.Id = list.Count > 0 ? list.Max(n => n.Id) + 1 : 1;
        if (string.IsNullOrEmpty(item.Date))
            item.Date = DateTime.Now.ToString("yyyy-MM-dd");
        list.Add(item);
        _cache = list;
        Save();
        return item;
    }

    public NewsItem? Update(NewsItem item)
    {
        var list = Load();
        var existing = list.FirstOrDefault(n => n.Id == item.Id);
        if (existing == null) return null;

        existing.Title = item.Title;
        existing.Summary = item.Summary;
        existing.Content = item.Content;
        existing.VideoUrl = item.VideoUrl;
        existing.Date = item.Date;
        Save();
        return existing;
    }

    public bool Delete(int id)
    {
        var list = Load();
        var item = list.FirstOrDefault(n => n.Id == id);
        if (item == null) return false;

        list.Remove(item);
        _cache = list;
        Save();
        return true;
    }
}
