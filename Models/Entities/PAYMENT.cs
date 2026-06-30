using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models.Entities
{
    [Table("PAYMENT")]
    public class PAYMENT
    {
        [Key]
        [Column("PaymentId")]
        public int PaymentId { get; set; }

        [StringLength(100)]
        [Column("TransactionId")]
        public string? TransactionId { get; set; }

        [Required]
        [Column("BookingId")]
        public int BookingId { get; set; }

        [Required]
        [Column("Amount", TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        [Column("PaymentMethod")]
        public string PaymentMethod { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("PaymentStatus")]
        public string PaymentStatus { get; set; } = "Pending";

        [Column("PaymentDate")]
        public DateTime? PaymentDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        [Column("ReferenceNumber")]
        public string? ReferenceNumber { get; set; }

        [StringLength(500)]
        [Column("Notes")]
        public string? Notes { get; set; }

        [ForeignKey("BookingId")]
        public virtual BOOKING? Booking { get; set; }
    }
}
