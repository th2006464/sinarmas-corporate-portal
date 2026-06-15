using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portal.Services;

namespace Portal.Pages.Admin;

[Authorize(Roles = "Admin")]
[IgnoreAntiforgeryToken]
public class NewsFormModel : PageModel
{
    private readonly NewsService _newsService;

    [BindProperty]
    public int Id { get; set; }

    [BindProperty]
    public string Title { get; set; } = "";

    [BindProperty]
    public string Summary { get; set; } = "";

    [BindProperty]
    public string NewsContent { get; set; } = "";

    [BindProperty]
    public string Date { get; set; } = "";

    [BindProperty]
    public string VideoUrl { get; set; } = "";

    public bool IsEdit { get; set; }

    public NewsFormModel(NewsService newsService)
    {
        _newsService = newsService;
    }

    public IActionResult OnGet(int? id)
    {
        if (id.HasValue)
        {
            IsEdit = true;
            var item = _newsService.GetById(id.Value);
            if (item == null) return RedirectToPage("/Admin/Dashboard");

            Id = item.Id;
            Title = item.Title;
            Summary = item.Summary;
            NewsContent = item.Content;
            Date = item.Date;
            VideoUrl = item.VideoUrl;
        }
        else
        {
            IsEdit = false;
            Date = DateTime.Now.ToString("yyyy-MM-dd");
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(NewsContent))
        {
            ModelState.AddModelError("", "标题和内容不能为空");
            return Page();
        }

        if (IsEdit)
        {
            _newsService.Update(new NewsItem
            {
                Id = Id,
                Title = Title,
                Summary = Summary,
                Content = NewsContent,
                Date = Date,
                VideoUrl = VideoUrl
            });
        }
        else
        {
            _newsService.Add(new NewsItem
            {
                Title = Title,
                Summary = Summary,
                Content = NewsContent,
                Date = Date,
                VideoUrl = VideoUrl
            });
        }

        return RedirectToPage("/Admin/Dashboard");
    }
}
