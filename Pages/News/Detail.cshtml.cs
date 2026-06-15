using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Services;

namespace Portal.Pages.News;

public class DetailModel : PageModel
{
    private readonly NewsService _newsService;
    public NewsItem? Item { get; set; }

    public DetailModel(NewsService newsService)
    {
        _newsService = newsService;
    }

    public IActionResult OnGet(int id)
    {
        Item = _newsService.GetById(id);
        if (Item == null) return NotFound();
        return Page();
    }
}
