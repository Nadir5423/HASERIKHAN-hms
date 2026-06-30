using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Staff
{
    public class CreateModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public CreateModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HOTEL_STAFF Staff { get; set; } = new();

        public IActionResult OnGet()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if username already exists
            if (await _context.HotelStaffs.AnyAsync(s => s.Username == Staff.Username))
            {
                ModelState.AddModelError("Staff.Username", "Username is already taken.");
                return Page();
            }

            Staff.IsActive = true;
            Staff.CreatedAt = DateTime.Now;
            Staff.UpdatedAt = DateTime.Now;

            _context.HotelStaffs.Add(Staff);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Staff member added successfully!";
            return RedirectToPage("./Index");
        }
    }
}
