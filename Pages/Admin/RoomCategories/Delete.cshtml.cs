using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Core;
using HMS.Data;
using HMS.Models.Entities;

namespace HMS.Pages.Admin.RoomCategories
{
    public class DeleteModel : BasePageModel
    {
        private readonly ICrudService<ROOM_CATEGORY> _service;
        private readonly HotelDbContext _context;

        public DeleteModel(ICrudService<ROOM_CATEGORY> service, HotelDbContext context)
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
            var category = await _service.GetByIdAsync(Category.CategoryId);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToPage("./Index");
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
                    Message = "This room category has active bookings on one or more of its rooms. Please cancel the booking(s) first before deleting the room category.",
                    ConfirmButtonText = "OK",
                    ShowCancel = false
                };
                TempData["DialogModel"] = dialog;
                return Page();
            }

            // Guard: check if any rooms belong to this category (excluding deleted rooms if possible, but actually any active room)
            var hasRooms = await _context.Rooms.AnyAsync(r => r.CategoryId == Category.CategoryId && r.Status != "Deleted");
            if (hasRooms)
            {
                var dialog = new HMS.Models.ViewModels.DialogModel
                {
                    Title = "Associated Rooms Exist",
                    Message = "There are active rooms associated with this category. Please change the category of those rooms first before deleting the category.",
                    ConfirmButtonText = "OK",
                    ShowCancel = false
                };
                TempData["DialogModel"] = dialog;
                return Page();
            }

            try
            {
                await _service.DeleteAsync(Category.CategoryId);
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            catch
            {
                TempData["ErrorMessage"] = "Cannot delete category. There are active dependencies.";
            }

            return RedirectToPage("./Index");
        }
    }
}
