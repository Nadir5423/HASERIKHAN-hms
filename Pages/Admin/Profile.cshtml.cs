using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin
{
    public class ProfileModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public ProfileModel(HotelDbContext context)
        {
            _context = context;
        }

        public HOTEL_STAFF Staff { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureUserType("Staff");
            if (authResult != null) return authResult;

            var staffId = CurrentUserId;
            if (!staffId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            var staff = await _context.HotelStaffs.FirstOrDefaultAsync(s => s.StaffId == staffId.Value);

            if (staff == null)
            {
                return NotFound();
            }

            Staff = staff;
            return Page();
        }
    }
}
