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
    public class DetailsModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public DetailsModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

      public Leaverequest Leaverequest { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var userId = HttpContext.Session.GetInt32("userId");

            // If there is no userID, the user needs to log in
            if (userId == null)
            {
                return RedirectToPage("/403");
            }

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
    }
}
