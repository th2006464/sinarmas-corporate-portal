using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Portal.Pages.Admin;

[IgnoreAntiforgeryToken]
public class LoginModel : PageModel
{
    private readonly IConfiguration _config;

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    public string? Error { get; set; }

    public LoginModel(IConfiguration config)
    {
        _config = config;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var adminUser = _config["Admin:Username"] ?? "admin";
        var adminPass = _config["Admin:Password"] ?? "admin123";

        if (Username == adminUser && Password == adminPass)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Username),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToPage("/Admin/Dashboard");
        }

        Error = "账号或密码错误";
        return Page();
    }
}
