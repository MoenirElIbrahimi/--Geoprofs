using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Leaverequests
{
    public class DetailsModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public DetailsModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }
        public Role UserRole { get; set; }

        public Leaverequest Leaverequest { get; set; }
        public Employee Employee { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var userId = HttpContext.Session.GetInt32("userId");

            // If there is no userID, the user needs to log in
            if (userId == null)
            {
                return RedirectToPage("/403");
            }

            if (id == null || _context.Leaverequest == null)
            {
                return NotFound();
            }

            var currentUser = await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.Team)
                .FirstOrDefaultAsync(e => e.ID == userId);

            // Controleer of de currentUser null is voordat je verder gaat
            if (currentUser == null)
            {
                RedirectToPage("/403");
            }

            UserRole = currentUser.Role;

            var leaverequest = await _context.Leaverequest
                .Include(l => l.Status)
                .Include(l => l.Category)
                .Include(l => l.Employee)
                    .ThenInclude(e => e.Role)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (leaverequest == null)
            {
                return NotFound();
            }
            else
            {
                Leaverequest = leaverequest;
                Employee = leaverequest.Employee; // Set the Employee property

                // Assuming 'Team' is a property of 'Employee'
                if (Employee != null && Employee.Team != null)
                {
                    // Now, you can access 'Team' through the 'Employee' property
                    var team = Employee.Team;
                }
            }

            return Page();

        }

    }
}
