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
using static ContosoUniversity.Models.Leaverequest;

namespace ContosoUniversity.Pages.Leaverequests
{
    public class CreateModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Leaverequest Leaverequest { get; set; }

        [BindProperty]
        public int LeaverequestCategory { get; set; }

        public List<Category> Categorys { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Categorys = await _context.Categorys.ToListAsync();

            // Get the userId from the session
            var userId = HttpContext.Session.GetInt32("userId") ?? default;

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

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Categorys = await _context.Categorys.ToListAsync();

            // set leaverequest category
            Leaverequest.Category = await _context.Categorys.FirstOrDefaultAsync(c => c.ID == LeaverequestCategory);

            TempData["SelectedCategory"] = LeaverequestCategory;

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

            // Check if Category is sick
            if (Leaverequest.Category.Name == "Sick")
            {
                // Check if sickleave is for today, if not return
                if (Leaverequest.StartDate.Date != DateTime.Today)
                {
                    ModelState.AddModelError(string.Empty, "Can only submit sick leave for today");
                    return Page();
                }

                // Check if sickleave enddate is set to tomorrow, if not return
                if (Leaverequest.EndDate != DateTime.Today.AddDays(1))
                {
                    ModelState.AddModelError(string.Empty, "Can only submit sick leave for a single day");
                    return Page();
                }

                bool hasSickLeaveForToday = await _context.Leaverequest.AnyAsync(
                l => l.Employee.ID == currentUser.ID
                    && l.Category.Name == "Sick"
                    && l.StartDate.Date == DateTime.Today);

                if (hasSickLeaveForToday)
                {
                    ModelState.AddModelError(string.Empty, "Already submitted sick leave for today");
                    return Page();
                }
            }

            _context.Leaverequest.Add(Leaverequest);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Leave request submitted";

            return RedirectToPage("/leaverequests/index");
        }
    }
}