using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Core;
using HMS.Data;
using HMS.Models.Entities;

namespace HMS.Pages.Admin.RoomCategories
{
    public class EditModel : BasePageModel
    {
        private readonly ICrudService<ROOM_CATEGORY> _service;
        private readonly HotelDbContext _context;

        public EditModel(ICrudService<ROOM_CATEGORY> service, HotelDbContext context)
        {
            _service = service;
            _context = context;
        }

        [BindProperty]
        public ROOM_CATEGORY Category { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            var category = await _service.GetByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToPage("./Index");
            }

            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Guard: check if any rooms in this category are booked by someone
            var hasBookings = await _context.Bookings.AnyAsync(b => 
                b.Room!.CategoryId == Category.CategoryId && 
                b.BookingStatus != Constants.BookingStatus.Cancelled && 
                b.BookingStatus != Constants.BookingStatus.CheckedOut);
            if (hasBookings)
            {
                var dialog = new HMS.Models.ViewModels.DialogModel
                {
                    Title = "Active Bookings Exist",
                    Message = "This room category has active bookings on one or more of its rooms. Please cancel the booking(s) first before updating the room category.",
                    ConfirmButtonText = "OK",
                    ShowCancel = false
                };
                TempData["DialogModel"] = dialog;
                return Page();
            }

            var category = await _service.GetByIdAsync(Category.CategoryId);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToPage("./Index");
            }

            category.CategoryName = Category.CategoryName;
            category.BasePrice = Category.BasePrice;
            category.MaxOccupancy = Category.MaxOccupancy;
            category.Description = Category.Description;
            category.UpdatedAt = DateTime.Now;

            await _service.UpdateAsync(category);
            TempData["SuccessMessage"] = "Category updated successfully!";
            return RedirectToPage("./Index");
        }
    }
}
