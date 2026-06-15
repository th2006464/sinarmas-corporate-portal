using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Portal.Pages;

public class ContactModel : PageModel
{
    [BindProperty]
    public string? Name { get; set; }

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public string? Phone { get; set; }

    [BindProperty]
    public string? Message { get; set; }

    public bool MessageSent { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        // In production, save to database or send email
        MessageSent = true;
        return Page();
    }
}
