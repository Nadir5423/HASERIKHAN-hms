using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;
using HMS.Services;

namespace HMS.Pages.Admin.Rooms
{
    public class EditModel : BasePageModel
    {
        private readonly HotelDbContext _context;
        private readonly BookingService _bookingService;

        public EditModel(HotelDbContext context, BookingService bookingService)
        {
            _context = context;
            _bookingService = bookingService;
        }

        [BindProperty]
        public ROOM Room { get; set; } = new();

        [BindProperty]
        public string AllocationType { get; set; } = "Existing";

        [BindProperty]
        public int? SelectedCustomerId { get; set; }

        [BindProperty]
        public string? CustomerName { get; set; }

        [BindProperty]
        public string? CustomerEmail { get; set; }

        [BindProperty]
        public string? CustomerPhone { get; set; }

        [BindProperty]
        public string? CustomerAddress { get; set; }

        public SelectList Categories { get; set; } = null!;
        public SelectList Customers { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin);
            if (authResult != null) return authResult;

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToPage("./Index");
            }

            Room = room;

            var categoriesList = await _context.RoomCategories.ToListAsync();
            Categories = new SelectList(categoriesList, "CategoryId", "CategoryName");

            var customersList = await _context.Customers.Where(c => c.IsActive).ToListAsync();
            Customers = new SelectList(customersList, "CustomerId", "Name");

            var hasBookings = await _context.Bookings.AnyAsync(b => b.RoomId == Room.RoomId && b.BookingStatus != Constants.BookingStatus.Cancelled && b.BookingStatus != Constants.BookingStatus.CheckedOut);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "This room has active bookings. Editing its properties or changing its status is blocked until the booking is cancelled.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categoriesList = await _context.RoomCategories.ToListAsync();
                Categories = new SelectList(categoriesList, "CategoryId", "CategoryName");
                var customersList = await _context.Customers.Where(c => c.IsActive).ToListAsync();
                Customers = new SelectList(customersList, "CustomerId", "Name");
                return Page();
            }

            // Check if room number already exists for another room
            if (await _context.Rooms.AnyAsync(r => r.RoomNumber == Room.RoomNumber && r.RoomId != Room.RoomId && r.Status != "Deleted"))
            {
                ModelState.AddModelError("Room.RoomNumber", "Room number already exists.");
                var categoriesList = await _context.RoomCategories.ToListAsync();
                Categories = new SelectList(categoriesList, "CategoryId", "CategoryName");
                var customersList = await _context.Customers.Where(c => c.IsActive).ToListAsync();
                Customers = new SelectList(customersList, "CustomerId", "Name");
                return Page();
            }

            // Guard: check if the room has any active bookings
            var hasBookings = await _context.Bookings.AnyAsync(b => b.RoomId == Room.RoomId && b.BookingStatus != Constants.BookingStatus.Cancelled && b.BookingStatus != Constants.BookingStatus.CheckedOut);
            if (hasBookings)
            {
                var dialog = new HMS.Models.ViewModels.DialogModel
                {
                    Title = "Active Bookings Exist",
                    Message = "This room has active bookings. Please cancel the booking(s) first before editing its properties or status.",
                    ConfirmButtonText = "OK",
                    ShowCancel = false
                };
                TempData["DialogModel"] = dialog;

                var categoriesList = await _context.RoomCategories.ToListAsync();
                Categories = new SelectList(categoriesList, "CategoryId", "CategoryName");
                var customersList = await _context.Customers.Where(c => c.IsActive).ToListAsync();
                Customers = new SelectList(customersList, "CustomerId", "Name");
                return Page();
            }

            var dbRoom = await _context.Rooms.FindAsync(Room.RoomId);
            if (dbRoom == null)
            {
                TempData["ErrorMessage"] = "Room not found.";
                return RedirectToPage("./Index");
            }

            // Check if status is being changed to Occupied from Available
            if (dbRoom.Status == Constants.RoomStatus.Available && Room.Status == Constants.RoomStatus.Occupied)
            {
                CUSTOMER? customer = null;

                if (AllocationType == "Existing")
                {
                    if (!SelectedCustomerId.HasValue)
                    {
                        ModelState.AddModelError("SelectedCustomerId", "Please select an existing customer.");
                    }
                    else
                    {
                        customer = await _context.Customers.FindAsync(SelectedCustomerId.Value);
                        if (customer == null)
                        {
                            ModelState.AddModelError("SelectedCustomerId", "Selected customer was not found.");
                        }
                    }
                }
                else // AllocationType == "New"
                {
                    if (string.IsNullOrWhiteSpace(CustomerName))
                    {
                        ModelState.AddModelError("CustomerName", "Customer name is required.");
                    }
                    if (string.IsNullOrWhiteSpace(CustomerEmail))
                    {
                        ModelState.AddModelError("CustomerEmail", "Customer email is required.");
                    }
                    if (string.IsNullOrWhiteSpace(CustomerPhone))
                    {
                        ModelState.AddModelError("CustomerPhone", "Customer phone is required.");
                    }

                    if (ModelState.IsValid)
                    {
                        customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == CustomerEmail);
                        if (customer == null)
                        {
                            // Generate unique username based on email prefix
                            string baseUsername = CustomerEmail!.Split('@')[0];
                            string username = baseUsername;
                            int counter = 1;
                            while (await _context.Customers.AnyAsync(c => c.Username == username))
                            {
                                username = $"{baseUsername}{counter}";
                                counter++;
                            }

                            customer = new CUSTOMER
                            {
                                Name = CustomerName!,
                                Email = CustomerEmail!,
                                Phone = CustomerPhone!,
                                Address = CustomerAddress,
                                Username = username,
                                PasswordHash = "Password123", // Default plain-text password for compatibility
                                IsActive = true,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };
                            _context.Customers.Add(customer);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    var categoriesList = await _context.RoomCategories.ToListAsync();
                    Categories = new SelectList(categoriesList, "CategoryId", "CategoryName");
                    var customersList = await _context.Customers.Where(c => c.IsActive).ToListAsync();
                    Customers = new SelectList(customersList, "CustomerId", "Name");
                    return Page();
                }

                if (customer != null)
                {
                    try
                    {
                        // Create a booking manually and check it in
                        var booking = await _bookingService.CreateBookingAsync(
                            customer.CustomerId,
                            Room.RoomId,
                            DateTime.Today,
                            DateTime.Today.AddDays(1),
                            1,
                            "Allocated manually by Admin");

                        booking.BookingStatus = Constants.BookingStatus.CheckedIn;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, $"Failed to allocate room: {ex.GetBaseException().Message}");
                        var categoriesList = await _context.RoomCategories.ToListAsync();
                        Categories = new SelectList(categoriesList, "CategoryId", "CategoryName");
                        var customersList = await _context.Customers.Where(c => c.IsActive).ToListAsync();
                        Customers = new SelectList(customersList, "CustomerId", "Name");
                        return Page();
                    }
                }
            }

            dbRoom.RoomNumber = Room.RoomNumber;
            dbRoom.CategoryId = Room.CategoryId;
            dbRoom.Status = Room.Status;
            dbRoom.Floor = Room.Floor;
            dbRoom.Description = Room.Description;
            dbRoom.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Room updated successfully!";
            return RedirectToPage("./Index");
        }
    }
}
