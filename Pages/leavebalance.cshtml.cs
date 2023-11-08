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
            Employee employee;

            // Get userID from session
            var userId = HttpContext.Session.GetInt32("UserID");

            // If there is no userID, the user needs to log in
            if (userId == null)
            {
                return RedirectToPage("/Index");
            }

            // If no ID parameter it should use the userID from session to get leavebalans
            if (id == null)
            {
                // Get user with id from session
                var user = await _context.Users
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(m => m.ID == userId);

                // If there is no user/employee with that session ID, user doesnt have permission
                if (user == null || user.Employee == null)
                {
                    return RedirectToPage("/403");
                }

                employee = user.Employee;
            }
            else
            {
                // Get user from session
                var currentUser = await _context.Users
                .Include(u => u.Employee)
                .ThenInclude(e => e.Role)
                .Where(u => u.ID == userId)
                .FirstOrDefaultAsync();

                // Check if user exists
                if (currentUser != null && currentUser.Employee != null)
                {
                    // Checck if user has permission
                    if (currentUser.Employee.Role.Name != "Manager")
                    {
                        return RedirectToPage("/403");
                    }
                }

                // Get employee where user ID = parameter ID
                employee = await _context.Users
                    .Where(u => u.ID == id)
                    .Select(u => u.Employee)
                    .FirstOrDefaultAsync();

                // If there is no employee with that ID redirect to 404
                if (employee == null)
                {
                    return RedirectToPage("/404");
                }
            }

            Employee = employee;
            // Fetch the leave requests associated with the employee
            Leaverequests = await _context.Leaverequests
                .Include(lr => lr.Status) // Include the Status entity
                .Where(lr => lr.Employee.ID == employee.ID && lr.Status.Name == "Accepted") // Filter by the 'Accepted' status
                .ToListAsync();

            return Page();
        }

    }
}
