using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContosoUniversity.Pages.Leaverequests
{
    public class EditModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public EditModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Leaverequest Leaverequest { get; set; } = default!;

        public List<SelectListItem> StatusItems { get; set; }

        public Employee CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // If parameter id is not set return not found
            if (id == null)
            {
                return NotFound();
            }

            // Get leaverequest from id parameter
            Leaverequest = await _context.Leaverequest
                .Include(lr => lr.Status)
                .Include(lr => lr.Employee)
                .ThenInclude(e => e.Team)
                .FirstOrDefaultAsync(m => m.ID == id);

            // Return not found if there are no leaverequests with that id
            if (Leaverequest == null)
            {
                return NotFound();
            }

            // Get the userId from the session
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == null)
            {
                return RedirectToPage("/403");
            }
            CurrentUser = await _context.Employees
                .Where(u => u.ID == userId)
                .Include(e => e.Role)
                .Include(e => e.Team)
                .FirstOrDefaultAsync();
            if (CurrentUser == null || CurrentUser.Role == null || CurrentUser.Team == null)
            {
                return RedirectToPage("/leaverequests/index");
            } else if (CurrentUser.Role.Name != "Manager") {
                return RedirectToPage("/403");
            } else if (CurrentUser.ID == Leaverequest.Employee.ID)
            {
                return RedirectToPage("/403");
            } else if (CurrentUser.Team.ID != Leaverequest.Employee.Team.ID) {
                return NotFound();
            }

            StatusItems = _context.Statuses
                .Select(s => new SelectListItem { Value = s.ID.ToString(), Text = s.Name })
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Leaverequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaverequestExists(Leaverequest.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LeaverequestExists(int id)
        {
            return _context.Leaverequest.Any(e => e.ID == id);
        }
    }
}
