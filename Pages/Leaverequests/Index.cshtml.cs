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
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;



        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }



        public IList<Leaverequest> Leaverequest { get; set; } = default!;



        public IList<Leaverequest> LeaverequestTeam { get; set; } = default!;



        public async Task OnGetAsync()
        {
            // Get the userId from the session
            var userId = HttpContext.Session.GetInt32("UserId") ?? default;

            if (_context.Leaverequest != null)    
            {        
                Leaverequest = await _context.Leaverequest             
                    .Where(lr => lr.Employee.ID == userId)            
                    .ToListAsync();        
                var userTeam = (await _context.Employee.FirstOrDefaultAsync(e => e.ID == userId))?.Team;         
                LeaverequestTeam = await _context.Leaverequest             
                    .Where(lr => lr.Employee.Team == userTeam && lr.Employee.ID != userId)             
                    .ToListAsync();   
            }
        }
    }
}