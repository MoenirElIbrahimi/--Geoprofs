using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages
{
    public class leavebalanceModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public leavebalanceModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public Employee Employee { get; set; }
        public List<Leaverequest> Leaverequests { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Get userID from session
            var userId = HttpContext.Session.GetInt32("userId");

            // If there is no userID, the user needs to log in
            if (userId == null)
            {
                return RedirectToPage("/403");
            }

            // Get user with id from session
            var currentUser = await _context.Employees
                .Include(e => e.Role)
                .FirstOrDefaultAsync(m => m.ID == userId);

            if (currentUser == null)
            {
                return RedirectToPage("/403");
            }

            // If no ID parameter it should use the userID from session to get leavebalans
            if (id == null)
            {
                Employee = currentUser;
            }
            else
            {
                Employee = await _context.Employees
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (Employee == null)
                {
                    return RedirectToPage("/404");
                }

                if (currentUser.ID != Employee.ID && currentUser.Role.Name != "Manager") {
                    return RedirectToPage("/403");
                }
            }

            // Fetch the leave requests associated with the employee
            Leaverequests = await _context.Leaverequests
                .Include(lr => lr.Status) // Include the Status entity
                .Where(lr => lr.Employee.ID == Employee.ID && lr.Status.Name == "Accepted") // Filter by the 'Accepted' status
                .ToListAsync();

            return Page();
        }

    }
}
