using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Portal.Pages;

public class AboutModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Channel { get; set; }

    public void OnGet()
    {
        if (string.IsNullOrEmpty(Channel))
            Channel = "intro";
    }
}
