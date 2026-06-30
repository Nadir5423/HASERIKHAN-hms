using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Staff
{
    public class EditModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public EditModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HOTEL_STAFF Staff { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            var staff = await _context.HotelStaffs.FindAsync(id);
            if (staff == null)
            {
                TempData["ErrorMessage"] = "Staff member not found.";
                return RedirectToPage("./Index");
            }

            Staff = staff;
            // Clear password hash so it's not rendered on page
            Staff.PasswordHash = "";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Staff.PasswordHash))
            {
                ModelState.Remove("Staff.PasswordHash");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if username already exists for another user
            if (await _context.HotelStaffs.AnyAsync(s => s.Username == Staff.Username && s.StaffId != Staff.StaffId))
            {
                ModelState.AddModelError("Staff.Username", "Username is already taken.");
                return Page();
            }

            var dbStaff = await _context.HotelStaffs.FindAsync(Staff.StaffId);
            if (dbStaff == null)
            {
                TempData["ErrorMessage"] = "Staff member not found.";
                return RedirectToPage("./Index");
            }

            dbStaff.Name = Staff.Name;
            dbStaff.Role = Staff.Role;
            dbStaff.Username = Staff.Username;
            dbStaff.Email = Staff.Email;
            dbStaff.Phone = Staff.Phone;
            dbStaff.IsActive = Staff.IsActive;
            dbStaff.UpdatedAt = DateTime.Now;

            // Only update password if a new one is typed
            if (!string.IsNullOrEmpty(Staff.PasswordHash))
            {
                dbStaff.PasswordHash = Staff.PasswordHash;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Staff member updated successfully!";
            return RedirectToPage("./Index");
        }
    }
}
