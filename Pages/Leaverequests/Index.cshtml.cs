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

        public IList<Status> Statuses { get; set; } = new List<Status>();

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public IList<Leaverequest> Leaverequest { get; set; } = default!;

        public IList<Leaverequest> LeaverequestTeam { get; set; } = default!;

        public IList<Category> Category { get; set; } = default!;

        public Role UserRole { get; set; }

        public async Task OnGetAsync(
     DateTime? selectedDate,
    string selectedStatus,
    string selectedCategory,
    string selectedCategoryTeam,
    DateTime? selectedDateTeam,
    string selectedStatusTeam)
        {
            Category = await _context.GetCategoriesAsync();
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == default)
            {
                TempData["ErrorMessage"] = "User ID not found in the session.";
                RedirectToPage("/"); // Redirect and return to avoid further execution
                return;
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

            // Defineer de variabele 'query'
            var query = _context.Leaverequest.Where(lr => lr.Employee.ID == userId);

            // ... (andere logica die query gebruikt)

            Leaverequest = await query
                .Include(lr => lr.Status)
                .Include(lr => lr.Category)
                .ToListAsync();
            if (selectedDate.HasValue)
            {
                query = query.Where(lr => lr.StartDate.Date <= selectedDate.Value.Date &&
                                          selectedDate.Value.Date < lr.EndDate.Date);
            }
            if (!string.IsNullOrEmpty(selectedStatus))
            {
                query = query.Where(lr => lr.Status.Name == selectedStatus);
            }
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                query = query.Where(lr => lr.Category.Name == selectedCategory);
            }
            Leaverequest = await query
                .Include(lr => lr.Status)
                .Include(lr => lr.Category)
                .ToListAsync();
            if (UserRole.Name == "Manager")
            {
                query = _context.Leaverequest
                    .Include(lr => lr.Employee)
                    .ThenInclude(e => e.Team)
                    .Where(lr => lr.Employee.Team.ID == currentUser.Team.ID &&
                                  lr.Employee.ID != userId);

                if (selectedDateTeam.HasValue)
                {
                    query = query.Where(lr => lr.StartDate.Date <= selectedDateTeam.Value.Date &&
                                              selectedDateTeam.Value.Date < lr.EndDate.Date);
                }

                if (!string.IsNullOrEmpty(selectedStatusTeam))
                {
                    query = query.Where(lr => lr.Status.Name == selectedStatusTeam);
                }

                var categories = await _context.Categorys.ToListAsync();

                LeaverequestTeam = await query
                    .Include(lr => lr.Status)
                    .ToListAsync();

                if (LeaverequestTeam == null)
                {
                    LeaverequestTeam = new List<Leaverequest>();
                }
            }
            else // Apply team filters if the user is not a manager
            {
                var teamQuery = _context.Leaverequest
                    .Include(lr => lr.Employee)
                    .ThenInclude(e => e.Team)
                    .Where(lr => lr.Employee.Team.ID == currentUser.Team.ID &&
                                  lr.Employee.ID != userId);

                if (selectedDateTeam.HasValue)
                {
                    teamQuery = teamQuery.Where(lr => lr.StartDate.Date <= selectedDateTeam.Value.Date &&
                                                       selectedDateTeam.Value.Date < lr.EndDate.Date);
                }

                if (!string.IsNullOrEmpty(selectedStatusTeam))
                {
                    teamQuery = teamQuery.Where(lr => lr.Status.Name == selectedStatusTeam);
                }

                var categories = await _context.Categorys.ToListAsync();

                LeaverequestTeam = await teamQuery
                    .Include(lr => lr.Status)
                    .ToListAsync();

                if (LeaverequestTeam == null)
                {
                    LeaverequestTeam = new List<Leaverequest>();
                }
            }
            ViewData["SelectedDate"] = selectedDate?.ToString("yyyy-MM-dd");
            ViewData["SelectedStatus"] = selectedStatus;
            ViewData["SelectedDateTeam"] = selectedDateTeam?.ToString("yyyy-MM-dd");
            ViewData["SelectedStatusTeam"] = selectedStatusTeam;
            ViewData["SelectedCategory"] = selectedCategory;
            ViewData["SelectedCategoryTeam"] = selectedCategoryTeam;
            Statuses = await _context.GetStatusesAsync();
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
            SickLeave.Category = sickCategory;

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
