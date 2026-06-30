using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;
using HMS.Services;

namespace HMS.Pages.Customer
{
    public class MyBookingsModel : BasePageModel
    {
        private readonly HotelDbContext _context;
        private readonly BookingService _bookingService;

        public MyBookingsModel(HotelDbContext context, BookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        public IList<BOOKING> Bookings { get; set; } = new List<BOOKING>();

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Customer);
            if (authResult != null) return authResult;

            int customerId = CurrentUserId ?? 0;

            Bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Category)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Customer);
            if (authResult != null) return authResult;

            int customerId = CurrentUserId ?? 0;

            // Verify the booking belongs to this customer
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == id && b.CustomerId == customerId);
            if (booking == null)
            {
                TempData["ErrorMessage"] = "Booking not found or access denied.";
                return RedirectToPage("./MyBookings");
            }

            try
            {
                await _bookingService.CancelBookingAsync(id);
                TempData["SuccessMessage"] = "Your booking has been successfully cancelled.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage("./MyBookings");
        }
    }
}
