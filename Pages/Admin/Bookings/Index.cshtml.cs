using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Bookings
{
    public class IndexModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public IndexModel(HotelDbContext context)
        {
            _context = context;
        }

        public IList<BOOKING> Bookings { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin, Constants.Roles.Manager, Constants.Roles.Receptionist);
            if (authResult != null) return authResult;

            Bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .ThenInclude(r => r.Category)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return Page();
        }
    }
}
