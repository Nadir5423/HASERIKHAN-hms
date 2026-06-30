using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HMS.Core;
using HMS.Data;
using HMS.Models.Entities;

namespace HMS.Services
{
    public class BookingService
    {
        private readonly HotelDbContext _context;

        public BookingService(HotelDbContext context)
        {
            _context = context;
        }

        // Search available rooms
        public async Task<List<ROOM>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests, int? categoryId = null)
        {
            // First find all overlapping booking room IDs
            var bookedRoomIds = await _context.Bookings
                .Where(b => b.BookingStatus != Constants.BookingStatus.Cancelled &&
                            b.BookingStatus != Constants.BookingStatus.CheckedOut &&
                            b.CheckInDate < checkOut &&
                            b.CheckOutDate > checkIn)
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            // Find rooms not booked, under category, occupancy support, and not in maintenance
            var query = _context.Rooms
                .Include(r => r.Category)
                .Where(r => !bookedRoomIds.Contains(r.RoomId) && 
                            r.Status != Constants.RoomStatus.Maintenance &&
                            r.Status != "Deleted" &&
                            r.Category!.MaxOccupancy >= guests);

            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            return await query.ToListAsync();
        }

        // Create booking
        public async Task<BOOKING> CreateBookingAsync(int customerId, int roomId, DateTime checkIn, DateTime checkOut, int guests, string? specialRequests, List<int>? serviceIds = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var room = await _context.Rooms
                    .Include(r => r.Category)
                    .FirstOrDefaultAsync(r => r.RoomId == roomId);

                if (room == null)
                    throw new Exception("Room not found");

                // Check overlap
                var overlapExists = await _context.Bookings
                    .AnyAsync(b => b.RoomId == roomId &&
                                   b.BookingStatus != Constants.BookingStatus.Cancelled &&
                                   b.BookingStatus != Constants.BookingStatus.CheckedOut &&
                                   b.CheckInDate < checkOut &&
                                   b.CheckOutDate > checkIn);

                if (overlapExists)
                    throw new Exception("Room is already booked for these dates.");

                // Calculate room price
                int nights = (checkOut - checkIn).Days;
                if (nights <= 0) nights = 1;
                decimal roomTotal = room.Category!.BasePrice * nights;

                // Calculate services total
                decimal servicesTotal = 0m;
                List<SERVICE> selectedServices = new();
                if (serviceIds != null && serviceIds.Count > 0)
                {
                    selectedServices = await _context.Services
                        .Where(s => serviceIds.Contains(s.ServiceId) && s.IsAvailable == true)
                        .ToListAsync();
                    servicesTotal = selectedServices.Sum(s => s.Price);
                }

                decimal totalAmount = roomTotal + servicesTotal;

                // Generate booking reference
                var lastBookingId = await _context.Bookings
                    .OrderByDescending(b => b.BookingId)
                    .Select(b => b.BookingId)
                    .FirstOrDefaultAsync();
                
                string bookingReference = $"HMS{(lastBookingId + 1).ToString("D5")}";

                var booking = new BOOKING
                {
                    BookingReference = bookingReference,
                    CustomerId = customerId,
                    RoomId = roomId,
                    CheckInDate = checkIn.Date,
                    CheckOutDate = checkOut.Date,
                    TotalAmount = totalAmount,
                    BookingStatus = Constants.BookingStatus.Pending,
                    NumberOfGuests = guests,
                    SpecialRequests = specialRequests,
                    BookingDate = DateTime.Now
                };

                _context.Bookings.Add(booking);
                room.Status = Constants.RoomStatus.Reserved;
                await _context.SaveChangesAsync();

                // Insert booking services
                foreach (var service in selectedServices)
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
                }

                if (selectedServices.Count > 0)
                    await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Confirm booking
        public async Task ConfirmBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
                throw new Exception("Booking not found");

            booking.BookingStatus = Constants.BookingStatus.Confirmed;
            await _context.SaveChangesAsync();
        }

        // Cancel booking
        public async Task CancelBookingAsync(int bookingId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                    throw new Exception("Booking not found");

                booking.BookingStatus = Constants.BookingStatus.Cancelled;
                booking.CancelledDate = DateTime.Now;

                // If they were checked in or reserved, make the room available again
                if (booking.Room != null && (booking.Room.Status == Constants.RoomStatus.Occupied || booking.Room.Status == Constants.RoomStatus.Reserved))
                {
                    booking.Room.Status = Constants.RoomStatus.Available;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Process Check-In
        public async Task ProcessCheckInAsync(int bookingId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                    throw new Exception("Booking not found");

                booking.BookingStatus = Constants.BookingStatus.CheckedIn;
                
                if (booking.Room != null)
                {
                    booking.Room.Status = Constants.RoomStatus.Occupied;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Process Check-Out (Generate invoice and payment in one transaction)
        public async Task ProcessCheckOutAsync(int bookingId, string paymentMethod, string? referenceNumber, string? paymentNotes)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Room)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                    throw new Exception("Booking not found");

                // 1. Complete Booking Status
                booking.BookingStatus = Constants.BookingStatus.CheckedOut;
                if (booking.Room != null)
                {
                    booking.Room.Status = Constants.RoomStatus.Available;
                }

                // 2. Generate Invoice
                decimal subtotal = booking.TotalAmount;
                decimal taxRate = 10.00m;
                decimal taxAmount = subtotal * (taxRate / 100m);
                decimal grandTotal = subtotal + taxAmount;

                string invoiceNumber = $"INV-{booking.BookingReference}";

                var invoice = new INVOICE
                {
                    BookingId = bookingId,
                    InvoiceNumber = invoiceNumber,
                    IssueDate = DateTime.Now,
                    Subtotal = subtotal,
                    TaxRate = taxRate,
                    TaxAmount = taxAmount,
                    TotalAmount = grandTotal,
                    Notes = $"Invoice generated on check-out. Thank you for staying with us!"
                };

                _context.Invoices.Add(invoice);

                // 3. Record Payment
                var payment = new PAYMENT
                {
                    BookingId = bookingId,
                    Amount = grandTotal,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = Constants.PaymentStatus.Paid,
                    PaymentDate = DateTime.Now,
                    TransactionId = $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                    ReferenceNumber = referenceNumber,
                    Notes = paymentNotes
                };

                _context.Payments.Add(payment);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
