using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Rooms
{
    public class CreateModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public CreateModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ROOM Room { get; set; } = new();

        public SelectList Categories { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            var categoriesList = await _context.RoomCategories.ToListAsync();
            Categories = new SelectList(categoriesList, "CategoryId", "CategoryName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categoriesList = await _context.RoomCategories.ToListAsync();
                Categories = new SelectList(categoriesList, "CategoryId", "CategoryName");
                return Page();
            }

            // Check if room number already exists
            if (await _context.Rooms.AnyAsync(r => r.RoomNumber == Room.RoomNumber && r.Status != "Deleted"))
            {
                ModelState.AddModelError("Room.RoomNumber", "Room number already exists.");
                var categoriesList = await _context.RoomCategories.ToListAsync();
                Categories = new SelectList(categoriesList, "CategoryId", "CategoryName");
                return Page();
            }

            Room.CreatedAt = DateTime.Now;
            Room.UpdatedAt = DateTime.Now;

            _context.Rooms.Add(Room);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Room added successfully!";
            return RedirectToPage("./Index");
        }
    }
}
