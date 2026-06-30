using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;
using HMS.Services;

namespace HMS.Pages.Admin.Bookings
{
    public class DetailsModel : BasePageModel
    {
        private readonly HotelDbContext _context;
        private readonly BookingService _bookingService;

        public DetailsModel(HotelDbContext context, BookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        public BOOKING Booking { get; set; } = default!;

        [BindProperty]
        public string PaymentMethod { get; set; } = "Credit Card";
        [BindProperty]
        public string? ReferenceNumber { get; set; }
        [BindProperty]
        public string? PaymentNotes { get; set; }

        // New property to hold all available services for admin to add
        public IList<SERVICE> AllServices { get; set; } = new List<SERVICE>();

        // Bind selected service IDs from the admin form
        [BindProperty]
        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin, Constants.Roles.Manager, Constants.Roles.Receptionist);
            if (authResult != null) return authResult;

            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .ThenInclude(r => r.Category)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .Include(b => b.Payments)
                .Include(b => b.Invoices)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            Booking = booking;

            // Load all available services for the add-services UI
            AllServices = await _context.Services
                .Where(s => s.IsAvailable && !s.ServiceName.Contains("_Del_"))
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddServicesAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin, Constants.Roles.Manager, Constants.Roles.Receptionist);
            if (authResult != null) return authResult;

            var booking = await _context.Bookings
                .Include(b => b.BookingServices)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null) return NotFound();

            if (SelectedServiceIds != null && SelectedServiceIds.Any())
            {
                var servicesToAdd = await _context.Services
                    .Where(s => SelectedServiceIds.Contains(s.ServiceId) && s.IsAvailable)
                    .ToListAsync();

                decimal addedServicesTotal = 0m;
                foreach (var service in servicesToAdd)
                {
                    // Prevent duplicate service entries
                    if (!booking.BookingServices.Any(bs => bs.ServiceId == service.ServiceId))
                    {
                        var bookingService = new BOOKING_SERVICE
                        {
                            BookingId = booking.BookingId,
                            ServiceId = service.ServiceId,
                            Quantity = 1,
                            TotalPrice = service.Price,
                            ServiceDate = DateTime.Now
                        };
                        _context.BookingServices.Add(bookingService);
                        addedServicesTotal += service.Price;
                    }
                }

                if (addedServicesTotal > 0)
                {
                    booking.TotalAmount += addedServicesTotal;
                    _context.Entry(booking).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Selected services added to booking and total amount updated.";
            }
            else
            {
                TempData["ErrorMessage"] = "No services selected.";
            }

            return RedirectToPage("./Details", new { id });
        }

        public async Task<IActionResult> OnPostRemoveServiceAsync(int id, int bookingServiceId)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin, Constants.Roles.Manager, Constants.Roles.Receptionist);
            if (authResult != null) return authResult;

            var booking = await _context.Bookings
                .Include(b => b.BookingServices)
                .FirstOrDefaultAsync(b => b.BookingId == id);
            if (booking == null) return NotFound();

            var bookingService = await _context.BookingServices.FindAsync(bookingServiceId);
            if (bookingService != null && bookingService.BookingId == id)
            {
                booking.TotalAmount -= bookingService.TotalPrice;
                if (booking.TotalAmount < 0) booking.TotalAmount = 0;

                _context.BookingServices.Remove(bookingService);
                _context.Entry(booking).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Service removed from booking and total amount updated.";
            }
            else
            {
                TempData["ErrorMessage"] = "Service not found in this booking.";
            }

            return RedirectToPage("./Details", new { id });
        }

        public async Task<IActionResult> OnPostConfirmAsync(int id)
        {
            try
            {
                await _bookingService.ConfirmBookingAsync(id);
                TempData["SuccessMessage"] = "Booking confirmed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.GetBaseException().Message;
            }
            return RedirectToPage("./Details", new { id });
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            try
            {
                await _bookingService.CancelBookingAsync(id);
                TempData["SuccessMessage"] = "Booking cancelled successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.GetBaseException().Message;
            }
            return RedirectToPage("./Details", new { id });
        }

        public async Task<IActionResult> OnPostCheckInAsync(int id)
        {
            try
            {
                await _bookingService.ProcessCheckInAsync(id);
                TempData["SuccessMessage"] = "Guest checked in successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.GetBaseException().Message;
            }
            return RedirectToPage("./Details", new { id });
        }

        public async Task<IActionResult> OnPostCheckOutAsync(int id)
        {
            try
            {
                await _bookingService.ProcessCheckOutAsync(id, PaymentMethod, ReferenceNumber, PaymentNotes);
                TempData["SuccessMessage"] = "Guest checked out and payment processed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.GetBaseException().Message;
            }
            return RedirectToPage("./Details", new { id });
        }
    }
}
