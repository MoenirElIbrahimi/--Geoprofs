using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using System.Runtime.CompilerServices;

namespace ContosoUniversity.Pages.Leaverequests
{
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;



        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }



        public IList<Leaverequest> Leaverequest { get; set; } = default!;

        public IList<Leaverequest> LeaverequestTeam { get; set; } = default!;

        public Role UserRole { get; set; }



        public async Task OnGetAsync()
        {
            // Get the userId from the session
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == default)
            {
                TempData["ErrorMessage"] = "User ID not found in the session.";
                RedirectToPage("/");
            }

            var currentUser = await _context.Employees
                    .Include(e => e.Role)
                    .Include(e => e.Team)
                    .FirstOrDefaultAsync(e => e.ID == userId);
            if (currentUser == null)
            {
                RedirectToPage("/403");
            }

            UserRole = currentUser.Role;

            if (_context.Leaverequest != null)    
            {        
                Leaverequest = await _context.Leaverequest             
                    .Where(lr => lr.Employee.ID == userId)   
                    .Include(lr => lr.Status)
                    .ToListAsync();

                LeaverequestTeam = await _context.Leaverequest
                    .Include(lr => lr.Employee)
                    .ThenInclude(e => e.Team)
                    .Where(lr => lr.Employee.Team.ID == currentUser.Team.ID && lr.Employee.ID != userId)
                    .Include(lr => lr.Status)
                    .ToListAsync();
                if (LeaverequestTeam == null)
                {
                    RedirectToPage("/404");
                }
            }
        }

        public Leaverequest SickLeave { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == default)
            {
                TempData["ErrorMessage"] = "User ID not found in the session.";
                return RedirectToPage("/");
            }

            var currentUser = await _context.Employee
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (currentUser == null)
            {
                return RedirectToPage("/403");
            }

            SickLeave = new Leaverequest();

            SickLeave.Employee = currentUser;

            var firstStatus = await _context.Statuses.FirstOrDefaultAsync();
            SickLeave.Status = firstStatus;

            var sickCategory = await _context.Categorys.FirstOrDefaultAsync(s => s.Name == "Sick");
            SickLeave.Type = sickCategory;

            DateTime today = DateTime.Today;

            SickLeave.StartDate = today;

            DateTime tomorrow = DateTime.Today.AddDays(1);

            SickLeave.EndDate = tomorrow;

            SickLeave.Reason = "Not applicable";

            _context.Leaverequest.Add(SickLeave);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sick leave submitted";

            return RedirectToPage("/leaverequests/index");
        }
    }
}