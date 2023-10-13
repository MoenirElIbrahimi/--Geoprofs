using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;



namespace ContosoUniversity.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;



        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
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
            // Hardcoded login credentials
            string managerEmail = "manager@geoprofs.com";
            string managerPassword = "manager";
            string werknemerEmail = "werknemer@geoprofs.com";
            string werknemerPassword = "werknemer";



            if ((Email == managerEmail && Password == managerPassword) || (Email == werknemerEmail && Password == werknemerPassword))
            {
                // Passwords match, you can log in the employee here
                if (Email == managerEmail)
                {
                    HttpContext.Session.SetInt32("UserID", 1);
                }
                else if (Email == werknemerEmail)
                {
                    HttpContext.Session.SetInt32("UserID", 2);
                }



                // Redirect to the leaverequests page
                return RedirectToPage("/leaverequests/index");
            }
            else
            {
                // Invalid email or password
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }
        }
    }
}