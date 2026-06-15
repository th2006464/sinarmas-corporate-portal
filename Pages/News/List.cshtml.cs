using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Services;

namespace Portal.Pages.News;

public class ListModel : PageModel
{
    private readonly NewsService _newsService;
    public List<NewsItem>? News { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    private const int PageSize = 10;

    public ListModel(NewsService newsService)
    {
        _newsService = newsService;
    }

    public void OnGet([FromQuery] int page = 1)
    {
        if (page < 1) page = 1;
        var (items, totalPages) = _newsService.GetPaged(page, PageSize);
        News = items;
        CurrentPage = page;
        TotalPages = totalPages;
    }
}
