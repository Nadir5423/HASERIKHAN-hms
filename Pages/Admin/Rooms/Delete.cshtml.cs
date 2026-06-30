using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin.Rooms
{
    public class DeleteModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public DeleteModel(HotelDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ROOM Room { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            var room = await _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToPage("./Index");
            }

            Room = room;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var room = await _context.Rooms.FindAsync(Room.RoomId);
            if (room == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToPage("./Index");
            }

            // Guard: prevent deletion if room has active bookings
            var hasBookings = await _context.Bookings.AnyAsync(b => b.RoomId == Room.RoomId && b.BookingStatus != Constants.BookingStatus.Cancelled && b.BookingStatus != Constants.BookingStatus.CheckedOut);
            if (hasBookings)
            {
                var dialog = new HMS.Models.ViewModels.DialogModel
                {
                    Title = "Active Bookings Exist",
                    Message = "This room has active bookings. Please cancel the booking(s) first before deleting the room.",
                    ConfirmButtonText = "OK",
                    ShowCancel = false
                };
                TempData["DialogModel"] = dialog;
                return Page();
            }

            // Perform soft-delete
            room.Status = "Deleted";
            
            string deletedSuffix = $"_Del_{room.RoomId}";
            int maxNumberLength = 20 - deletedSuffix.Length;
            if (room.RoomNumber.Length > maxNumberLength)
            {
                room.RoomNumber = room.RoomNumber.Substring(0, maxNumberLength) + deletedSuffix;
            }
            else
            {
                room.RoomNumber = room.RoomNumber + deletedSuffix;
            }

            room.UpdatedAt = DateTime.Now;
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Room deleted successfully!";
            return RedirectToPage("./Index");
        }
    }
}

