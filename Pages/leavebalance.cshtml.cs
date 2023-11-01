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

            if (id == null)
            {
                var userId = HttpContext.Session.GetInt32("UserID");
                if (userId == null)
                {
                    return NotFound();
                }

                var user = await _context.Users
                    .Include(u => u.Employee) // Include the Employee data
                    .FirstOrDefaultAsync(m => m.ID == userId);

                if (user == null || user.Employee == null)
                {
                    return NotFound();
                }

                employee = user.Employee;
            }
            else
            {
                employee = await _context.Employees
                    .FirstOrDefaultAsync(m => m.ID == id);

                if (employee == null)
                {
                    return NotFound();
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
