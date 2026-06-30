using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models.Entities
{
    [Table("BOOKING")]
    public class BOOKING
    {
        [Key]
        [Column("BookingId")]
        public int BookingId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("BookingReference")]
        public string BookingReference { get; set; } = null!;

        [Required]
        [Column("CustomerId")]
        public int CustomerId { get; set; }

        [Required]
        [Column("RoomId")]
        public int RoomId { get; set; }

        [Required]
        [Column("CheckInDate", TypeName = "date")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Column("CheckOutDate", TypeName = "date")]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Column("TotalAmount", TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        [Column("BookingStatus")]
        public string BookingStatus { get; set; } = "Pending";

        [Required]
        [Column("NumberOfGuests")]
        public int NumberOfGuests { get; set; }

        [StringLength(500)]
        [Column("SpecialRequests")]
        public string? SpecialRequests { get; set; }

        [Column("BookingDate")]
        public DateTime? BookingDate { get; set; } = DateTime.Now;

        [Column("CancelledDate")]
        public DateTime? CancelledDate { get; set; }

        [ForeignKey("CustomerId")]
        public virtual CUSTOMER? Customer { get; set; }

        [ForeignKey("RoomId")]
        public virtual ROOM? Room { get; set; }

        public virtual ICollection<PAYMENT> Payments { get; set; } = new List<PAYMENT>();
        public virtual ICollection<INVOICE> Invoices { get; set; } = new List<INVOICE>();
        public virtual ICollection<BOOKING_SERVICE> BookingServices { get; set; } = new List<BOOKING_SERVICE>();
    }
}
