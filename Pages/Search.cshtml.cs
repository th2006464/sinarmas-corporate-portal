using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Services;

namespace Portal.Pages;

public class SearchModel : PageModel
{
    private readonly NewsService _newsService;

    [BindProperty(SupportsGet = true)]
    public string? Key { get; set; }

    public List<NewsItem> Results { get; set; } = new();

    public SearchModel(NewsService newsService)
    {
        _newsService = newsService;
    }

    public void OnGet()
    {
        if (!string.IsNullOrWhiteSpace(Key))
        {
            Results = _newsService.Search(Key);
        }
    }
}
