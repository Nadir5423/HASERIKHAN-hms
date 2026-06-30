using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data;
using HMS.Models.Entities;
using HMS.Core;

namespace HMS.Pages.Admin
{
    public class IndexModel : BasePageModel
    {
        private readonly HotelDbContext _context;

        public IndexModel(HotelDbContext context)
        {
            _context = context;
        }

        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int MaintenanceRooms { get; set; }
        public int ActiveBookings { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<BOOKING> RecentBookings { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var authResult = EnsureAuthorized(Constants.Roles.Admin, Constants.Roles.Manager, Constants.Roles.Receptionist);
            if (authResult != null) return authResult;

            TotalRooms = await _context.Rooms.CountAsync();
            AvailableRooms = await _context.Rooms.CountAsync(r => r.Status == Constants.RoomStatus.Available);
            OccupiedRooms = await _context.Rooms.CountAsync(r => r.Status == Constants.RoomStatus.Occupied);
            MaintenanceRooms = await _context.Rooms.CountAsync(r => r.Status == Constants.RoomStatus.Maintenance);

            ActiveBookings = await _context.Bookings.CountAsync(b => b.BookingStatus == Constants.BookingStatus.CheckedIn || b.BookingStatus == Constants.BookingStatus.Confirmed);

            TotalRevenue = await _context.Payments
                .Where(p => p.PaymentStatus == Constants.PaymentStatus.Paid)
                .SumAsync(p => p.Amount);

            RecentBookings = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToListAsync();

            return Page();
        }
    }
}
