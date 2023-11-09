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

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the userId from the session
            var userId = HttpContext.Session.GetInt32("UserId") ?? default;

            if (userId == default)
            {
                return RedirectToPage("/403");
            }

            // Check the role of the user in the database
            var currentUser = await _context.Employee
                .Include(e => e.Role)
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (currentUser == null)
            {
                return RedirectToPage("/403");
            }

            return Page();
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

            var userId = HttpContext.Session.GetInt32("userId") ?? default;

            if (userId == default)
            {
                return RedirectToPage("/404");
            }

            // Set user for leaverequest
            var currentUser = await _context.Employee
                .FirstOrDefaultAsync(u => u.ID == userId);

            if (currentUser == null)
            {
                return RedirectToPage("/403");
            }

            Leaverequest.Employee = currentUser;

            // Set the "Status" to 1
            var firstStatus = await _context.Statuses.FirstOrDefaultAsync();
            Leaverequest.Status = firstStatus;

            var firstCategory = await _context.Categorys.FirstOrDefaultAsync();
            Leaverequest.Type = firstCategory;
            
            _context.Leaverequest.Add(Leaverequest);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Leave request submitted";
            
            return RedirectToPage("/leaverequests/index");
        }
    }
}