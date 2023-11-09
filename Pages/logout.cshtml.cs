using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.Pages
{
    public class logoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Verwijder de "userID" en "loggedin" uit de sessie om uit te loggen.
            HttpContext.Session.Remove("userId");
            HttpContext.Session.Remove("loggedin");

            // Redirect naar de inlogpagina of een andere gewenste bestemming na uitloggen.
            return RedirectToPage("/index");
        }
    }
}
