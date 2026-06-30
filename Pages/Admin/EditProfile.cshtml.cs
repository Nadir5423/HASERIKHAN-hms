using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin
{
    public class EditProfileModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public EditProfileModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HOTEL_STAFF Staff { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureUserType("Staff");
            if (authResult != null) return authResult;

            var staffId = CurrentUserId;
            if (!staffId.HasValue) return RedirectToPage("/Account/Login");

            var staff = await _context.HotelStaffs.FindAsync(staffId.Value);
            if (staff == null) return NotFound();

            Staff = staff;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var authResult = EnsureUserType("Staff");
            if (authResult != null) return authResult;

            var staffId = CurrentUserId;
            if (!staffId.HasValue) return RedirectToPage("/Account/Login");

            // Remove validation for fields we don't edit via this form
            ModelState.Remove("Staff.PasswordHash");
            ModelState.Remove("Staff.Username");
            ModelState.Remove("Staff.Role");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var staffToUpdate = await _context.HotelStaffs.FindAsync(staffId.Value);
            if (staffToUpdate == null) return NotFound();

            // Validate that email is not taken by another staff member
            var emailExists = await _context.HotelStaffs.AnyAsync(s => s.Email == Staff.Email && s.StaffId != staffId.Value);
            if (emailExists)
            {
                ModelState.AddModelError("Staff.Email", "This email address is already in use by another staff member.");
                return Page();
            }

            staffToUpdate.Name = Staff.Name;
            staffToUpdate.Email = Staff.Email;
            staffToUpdate.Phone = Staff.Phone;
            staffToUpdate.UpdatedAt = System.DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToPage("./Profile");
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = ex.GetBaseException().Message;
                return Page();
            }
        }
    }
}
