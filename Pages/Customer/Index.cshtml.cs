using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;
using HMS.Services;

namespace HMS.Pages.Customer
{
    public class IndexModel : BasePageModel
    {
        private readonly HotelDbContext _context;
        private readonly BookingService _bookingService;

        public IndexModel(HotelDbContext context, BookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [BindProperty(SupportsGet = true)]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);

        [BindProperty(SupportsGet = true)]
        public int Guests { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();

        public List<ROOM> AvailableRooms { get; set; } = new();

        public bool HasSearched { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Allow guest users (not logged in) to browse and search rooms.
            // If logged in as a Staff member, redirect to the Admin index dashboard.
            if (IsLoggedIn && CurrentUserType == "Staff")
            {
                return RedirectToPage("/Admin/Index");
            }

            // Load categories for dropdown
            var cats = await _context.RoomCategories.ToListAsync();
            Categories = cats.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();

            // If it's a search (parameters are provided via GET form submission)
            if (Request.Query.ContainsKey("CheckInDate") && Request.Query.ContainsKey("CheckOutDate"))
            {
                HasSearched = true;

                if (CheckInDate < DateTime.Today)
                {
                    ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past.");
                }
                if (CheckOutDate <= CheckInDate)
                {
                    ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
                }
                if (Guests < 1)
                {
                    ModelState.AddModelError("Guests", "At least 1 guest is required.");
                }

                if (ModelState.IsValid)
                {
                    AvailableRooms = await _bookingService.GetAvailableRoomsAsync(CheckInDate, CheckOutDate, Guests, CategoryId);
                }
            }

            return Page();
        }
    }
}
