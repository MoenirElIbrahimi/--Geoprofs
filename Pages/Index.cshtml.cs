using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ContosoUniversity.Models;
using ContosoUniversity.Data;

namespace ContosoUniversity.Pages
{
    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly SchoolContext _context;

        public IndexModel(ILogger<IndexModel> logger, ContosoUniversity.Data.SchoolContext context)
        {
            _logger = logger;
            _context = context;
        }



        [BindProperty]
        public string Email { get; set; }



        [BindProperty]
        public string Password { get; set; }



        public void OnGet()
        {
            string userRole = HttpContext.Session.GetString("UserRole");



            if (userRole == "manager" || userRole == "werknemer")
            {
                // Redirect to the leaverequests page
                Response.Redirect("/leaverequests");
            }
        }



        public IActionResult OnPost()
        {
            // Je kunt de databasecontext hier gebruiken zonder deze expliciet in de methode te passen.
            var email = Email;
            var password = Password;

            // Voer de inlogcontrole uit
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // De inloggegevens zijn geldig. Sla de gebruikersinformatie op in de sessie.
                HttpContext.Session.SetInt32("UserID", user.ID);

                // Redirect naar de leaverequests-pagina of een andere beveiligde pagina.
                return RedirectToPage("/leaverequests/index");
            }
            else
            {
                // Ongeldige inloggegevens
                ModelState.AddModelError(string.Empty, "Ongeldige gebruikersnaam of wachtwoord.");
                return Page();
            }
        }


    }
}