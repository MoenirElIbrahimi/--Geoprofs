using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages.Leaverequests
{
    public class CreateModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public CreateModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            // Get the userId from the session
            var userId = HttpContext.Session.GetInt32("UserId") ?? default;



            // Check the role of the user in the database
            var user = await _context.Employee.FirstOrDefaultAsync(u => u.ID == userId);



            if (!(user != null && (user.Role == 1 || user.Role == 2)))
            {
                // Redirect to the Privacy page
                Response.Redirect("/");
            }
        }

        [BindProperty]
        public Leaverequest Leaverequest { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Leaverequest.StartDate > Leaverequest.EndDate)
            {
                ModelState.AddModelError(string.Empty, "Start date must be earlier than the end date.");
                return Page();
            }

            // Set the "Status" to 1
            Leaverequest.Status = 1;

            // Associate with the "Employee" with ID 1
            Leaverequest.Employee = _context.Employees.Find(1);

            _context.Leaverequest.Add(Leaverequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Leave request submitted";

            return RedirectToPage("./Index");
        }
    }
}
