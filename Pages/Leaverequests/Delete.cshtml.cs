using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Leaverequests
{
    public class DeleteModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public DeleteModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Leaverequest Leaverequest { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Leaverequest == null)
            {
                return NotFound();
            }

            var leaverequest = await _context.Leaverequest.FirstOrDefaultAsync(m => m.ID == id);

            if (leaverequest == null)
            {
                return NotFound();
            }
            else 
            {
                Leaverequest = leaverequest;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Leaverequest == null)
            {
                return NotFound();
            }
            var leaverequest = await _context.Leaverequest.FindAsync(id);

            if (leaverequest != null)
            {
                Leaverequest = leaverequest;
                _context.Leaverequest.Remove(Leaverequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
