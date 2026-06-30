using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Rooms
{
    public class IndexModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public IndexModel(HotelDbContext context)
        {
            _context = context;
        }

        public List<ROOM> Rooms { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            Rooms = await _context.Rooms
                .Include(r => r.Category)
                .Where(r => r.Status != "Deleted")
                .ToListAsync();

            return Page();
        }
    }
}
