using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Services
{
    public class IndexModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public IndexModel(HotelDbContext context)
        {
            _context = context;
        }

        public IList<SERVICE> Services { get; set; } = new List<SERVICE>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Only Admins and Managers can access this page
            var authResult = EnsureAuthorized(Constants.Roles.Admin, Constants.Roles.Manager);
            if (authResult != null) return authResult;

            Services = await _context.Services
                .Where(s => !s.ServiceName.Contains("_Del_"))
                .AsNoTracking()
                .ToListAsync();
            return Page();
        }
    }
}
