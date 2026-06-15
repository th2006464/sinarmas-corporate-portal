using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Services;

namespace Portal.Pages.Admin;

[Authorize(Roles = "Admin")]
[IgnoreAntiforgeryToken]
public class DashboardModel : PageModel
{
    private readonly NewsService _newsService;
    public List<NewsItem> News { get; set; } = new();

    public DashboardModel(NewsService newsService)
    {
        _newsService = newsService;
    }

    public void OnGet()
    {
        News = _newsService.GetAll();
    }

    public IActionResult OnPostDelete(int id)
    {
        _newsService.Delete(id);
        return RedirectToPage();
    }
}
