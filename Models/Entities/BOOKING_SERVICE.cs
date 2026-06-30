using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models.Entities
{
    [Table("BOOKING_SERVICES")]
    public class BOOKING_SERVICE
    {
        [Key]
        [Column("BookingServiceId")]
        public int BookingServiceId { get; set; }

        [Required]
        [Column("BookingId")]
        public int BookingId { get; set; }

        [Required]
        [Column("ServiceId")]
        public int ServiceId { get; set; }

        [Column("Quantity")]
        public int? Quantity { get; set; } = 1;

        [Required]
        [Column("TotalPrice", TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        [Column("ServiceDate")]
        public DateTime? ServiceDate { get; set; } = DateTime.Now;

        [ForeignKey("BookingId")]
        public virtual BOOKING? Booking { get; set; }

        [ForeignKey("ServiceId")]
        public virtual SERVICE? Service { get; set; }
    }
}
