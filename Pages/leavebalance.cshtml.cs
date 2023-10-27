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

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            if (_context.Employees == null)
            {

                return NotFound();
            }

            Employee employee;

            if (id == null)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null) {
                    return NotFound();
                }

                var user = await _context.Users.FirstOrDefaultAsync(m => m.ID == userId);
                if (user == null)
                {
                    return NotFound();
                }

                employee = user.Employee;

                if (employee == null)
                {
                    return NotFound();
                }
                else
                {
                    Employee = employee;
                }
                return Page();

            }

            employee = await _context.Employees.FirstOrDefaultAsync(m => m.ID == id);
            if (employee == null)
            {
                return NotFound();
            }
            else 
            {
                Employee = employee;
            }
            return Page();
        }
    }
}
