using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Services;

namespace Portal.Pages;

public class IndexModel : PageModel
{
    private readonly NewsService _newsService;
    public List<NewsItem> News { get; set; } = new();

    public IndexModel(NewsService newsService)
    {
        _newsService = newsService;
    }

    public void OnGet()
    {
        News = _newsService.GetLatest(6);
    }
}
