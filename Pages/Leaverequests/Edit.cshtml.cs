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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Leaverequest = await _context.Leaverequest
                .Include(lr => lr.Status)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Leaverequest == null)
            {
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
