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

        [BindProperty]
        public List<int> selectedIDS { get; set; } = new List<int>();

        [BindProperty]
        public int SelectedStatus { get; set; }

        public IList<Status> Statuses { get; set; } = new List<Status>();

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public IList<Leaverequest> Leaverequest { get; set; } = default!;

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public int CurrentTeamPage { get; set; }
        public int TotalTeamPages { get; set; }
        public bool HasPreviousTeamPage => CurrentTeamPage > 1;
        public bool HasNextTeamPage => CurrentTeamPage < TotalTeamPages;

        public IList<Leaverequest> LeaverequestTeam { get; set; } = default!;

        public IList<Category> Category { get; set; } = default!;

        public Role UserRole { get; set; }

        public async Task<IActionResult> OnGetAsync(
         DateTime? selectedDate,
        string selectedStatus,
        string selectedCategory,
        string selectedCategoryTeam,
        DateTime? selectedDateTeam,
        string selectedStatusTeam)
        {
            int pageSize = 10;

            var pageQueryParam = HttpContext.Request.Query["page"];
            int? page = string.IsNullOrEmpty(pageQueryParam) ? null : int.Parse(pageQueryParam);
            int currentPage = (page.HasValue && page > 0) ? page.Value : 1;

            var teamPageQueryParam = HttpContext.Request.Query["teamPage"];
            int? teamPage = string.IsNullOrEmpty(teamPageQueryParam) ? null : int.Parse(teamPageQueryParam);
            int currentTeamPage = (teamPage.HasValue && teamPage > 0) ? teamPage.Value : 1;

            Category = await _context.GetCategoriesAsync();
            var userId = HttpContext.Session.GetInt32("userId");
            if (userId == default || userId == null)
            {
                return RedirectToPage("/403");
            }

            var currentUser = await _context.Employees
                .Include(e => e.Role)
                .Include(e => e.Team)
                .FirstOrDefaultAsync(e => e.ID == userId);

            // Controleer of de currentUser null is voordat je verder gaat
            if (currentUser == null)
            {
                return RedirectToPage("/403");
            }

            UserRole = currentUser.Role;

            // Defineer de variabele 'query'
            var query = _context.Leaverequest.Where(lr => lr.Employee.ID == userId);

            Leaverequest = await query
                .Include(lr => lr.Status)
                .Include(lr => lr.Category)
                .Include(lr => lr.Employee)
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

            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(Leaverequest.Count / (double)pageSize);

            Leaverequest = Leaverequest
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();


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
                if (!string.IsNullOrEmpty(selectedCategoryTeam))
                {
                    query = query.Where(lr => lr.Category.Name == selectedCategoryTeam);
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
                if (!string.IsNullOrEmpty(selectedCategoryTeam))
                {
                    teamQuery = teamQuery.Where(lr => lr.Category.Name == selectedCategoryTeam);
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

            CurrentTeamPage = currentTeamPage;
            TotalTeamPages = (int)Math.Ceiling(LeaverequestTeam.Count / (double)pageSize);

            LeaverequestTeam = LeaverequestTeam
            .Skip((currentTeamPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            ViewData["SelectedDate"] = selectedDate?.ToString("yyyy-MM-dd");
            ViewData["SelectedStatus"] = selectedStatus;
            ViewData["SelectedDateTeam"] = selectedDateTeam?.ToString("yyyy-MM-dd");
            ViewData["SelectedStatusTeam"] = selectedStatusTeam;
            ViewData["SelectedCategory"] = selectedCategory;
            ViewData["SelectedCategoryTeam"] = selectedCategoryTeam;
            ViewData["page"] = page;
            ViewData["teamPage"] = teamPage;
            Statuses = await _context.GetStatusesAsync();

            return Page();
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

            DateTime today = DateTime.Today;

            bool hasSickLeaveForToday = await _context.Leaverequest.AnyAsync(
            l => l.Employee.ID == currentUser.ID
                 && l.Category.Name == "Sick"
                 && l.StartDate.Date == today);

            if (hasSickLeaveForToday)
            {
                TempData["ErrorMessage"] = "Already submitted sick leave for today";

                return RedirectToPage("/leaverequests/index");
            }

            SickLeave = new Leaverequest();

            SickLeave.Employee = currentUser;

            var firstStatus = await _context.Statuses.FirstOrDefaultAsync();
            SickLeave.Status = firstStatus;

            var sickCategory = await _context.Categorys.FirstOrDefaultAsync(s => s.Name == "Sick");
            SickLeave.Category = sickCategory;

            SickLeave.StartDate = today;

            DateTime tomorrow = DateTime.Today.AddDays(1);

            SickLeave.EndDate = tomorrow;

            SickLeave.Reason = "Not applicable";

            _context.Leaverequest.Add(SickLeave);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sick leave submitted";

            return RedirectToPage("/leaverequests/index");
        }

        public async Task<IActionResult> OnPostBulkEdit()
        {
            if (selectedIDS != null && selectedIDS.Count > 0)
            {
                foreach (var selectedID in selectedIDS)
                {
                    Leaverequest selectedLeaverequest = await _context.Leaverequests.Include(lr => lr.Status).FirstOrDefaultAsync(lr => lr.ID == selectedID);
                    if (selectedLeaverequest != null)
                    {
                        selectedLeaverequest.Status = await _context.Statuses.FirstOrDefaultAsync(s => s.ID == SelectedStatus);

                        // Mark the entity as modified (Entity Framework Core tracks changes)
                        _context.Leaverequests.Update(selectedLeaverequest);

                        // Save changes to your data source (e.g., database)
                        await _context.SaveChangesAsync();
                    }
                }
            }

            // Redirect to the same page or another page as needed
            TempData["SuccessMessage"] = "Updated statusses";

            return RedirectToPage("/leaverequests/index");
        }
    }
}
