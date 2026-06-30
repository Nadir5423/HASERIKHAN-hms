using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;
using HMS.Services;

namespace HMS.Pages.Customer
{
    public class BookRoomModel : BasePageModel
    {
        private readonly HotelDbContext _context;
        private readonly BookingService _bookingService;

        public BookRoomModel(HotelDbContext context, BookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [BindProperty(SupportsGet = true)]
        public int RoomId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime CheckIn { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime CheckOut { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Guests { get; set; }

        [BindProperty]
        public string? SpecialRequests { get; set; }

        public ROOM Room { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public int Nights { get; set; }
        public IList<SERVICE> Services { get; set; } = new List<SERVICE>();

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Customer);
            if (authResult != null) return authResult;

            var room = await _context.Rooms
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.RoomId == RoomId && r.Status != "Deleted");

            if (room == null || CheckIn >= CheckOut || CheckIn < DateTime.Today)
            {
                TempData["ErrorMessage"] = "Invalid booking parameters.";
                return RedirectToPage("./Index");
            }

            Room = room;
            Nights = (CheckOut - CheckIn).Days;
            if (Nights <= 0) Nights = 1;
            TotalAmount = Room.Category!.BasePrice * Nights;

            // Load available services for customer selection
            Services = await _context.Services.Where(s => s.IsAvailable).ToListAsync();

            return Page();
        }

        [BindProperty]
        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        public async Task<IActionResult> OnPostAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Customer);
            if (authResult != null) return authResult;

            try
            {
                // Customer ID from session
                int customerId = CurrentUserId ?? 0;

                var booking = await _bookingService.CreateBookingAsync(customerId, RoomId, CheckIn, CheckOut, Guests, SpecialRequests, SelectedServiceIds);
                
                TempData["SuccessMessage"] = $"Booking successful! Your reference is {booking.BookingReference}.";
                return RedirectToPage("./MyBookings");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.GetBaseException().Message;
                // Reload room
                Room = await _context.Rooms.Include(r => r.Category).FirstAsync(r => r.RoomId == RoomId);
                Nights = (CheckOut - CheckIn).Days;
                if (Nights <= 0) Nights = 1;
                TotalAmount = Room.Category!.BasePrice * Nights;
                // reload services for UI
                Services = await _context.Services.Where(s => s.IsAvailable).ToListAsync();
                return Page();
            }
        }
    }
}
