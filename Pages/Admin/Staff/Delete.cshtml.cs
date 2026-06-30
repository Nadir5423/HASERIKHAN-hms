using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Staff
{
    public class DeleteModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public DeleteModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HOTEL_STAFF Staff { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            Staff = await _context.HotelStaffs.FindAsync(id);

            if (Staff == null)
            {
                TempData["ErrorMessage"] = "Staff member not found.";
                return RedirectToPage("./Index");
            }

            // Prevent deleting the currently logged-in user
            if (Staff.Username == HttpContext.Session.GetString(Constants.SessionKeys.Username))
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            var staff = await _context.HotelStaffs.FindAsync(id);

            if (staff != null)
            {
                if (staff.Username == HttpContext.Session.GetString(Constants.SessionKeys.Username))
                {
                    TempData["ErrorMessage"] = "You cannot delete your own account.";
                    return RedirectToPage("./Index");
                }

                _context.HotelStaffs.Remove(staff);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Staff member deleted successfully.";
            }

            return RedirectToPage("./Index");
        }
    }
}
