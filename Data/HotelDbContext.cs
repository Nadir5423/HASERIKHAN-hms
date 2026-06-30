using Microsoft.EntityFrameworkCore;
using HMS.Models.Entities;

namespace HMS.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public virtual DbSet<ROOM_CATEGORY> RoomCategories { get; set; } = null!;
        public virtual DbSet<ROOM> Rooms { get; set; } = null!;
        public virtual DbSet<CUSTOMER> Customers { get; set; } = null!;
        public virtual DbSet<HOTEL_STAFF> HotelStaffs { get; set; } = null!;
        public virtual DbSet<BOOKING> Bookings { get; set; } = null!;
        public virtual DbSet<PAYMENT> Payments { get; set; } = null!;
        public virtual DbSet<INVOICE> Invoices { get; set; } = null!;
        public virtual DbSet<SERVICE> Services { get; set; } = null!;
        public virtual DbSet<BOOKING_SERVICE> BookingServices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          

            // Configure unique indexes
            modelBuilder.Entity<ROOM_CATEGORY>()
                .HasIndex(rc => rc.CategoryName)
                .IsUnique();

            modelBuilder.Entity<ROOM>()
                .HasIndex(r => r.RoomNumber)
                .IsUnique();

            modelBuilder.Entity<CUSTOMER>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<CUSTOMER>()
                .HasIndex(c => c.Username)
                .IsUnique();

            modelBuilder.Entity<HOTEL_STAFF>()
                .HasIndex(hs => hs.Username)
                .IsUnique();

            modelBuilder.Entity<BOOKING>()
                .HasIndex(b => b.BookingReference)
                .IsUnique();

            modelBuilder.Entity<PAYMENT>()
                .HasIndex(p => p.TransactionId)
                .IsUnique();

            modelBuilder.Entity<INVOICE>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            modelBuilder.Entity<SERVICE>()
                .HasIndex(s => s.ServiceName)
                .IsUnique();
        }
    }
}
