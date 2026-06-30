using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models.Entities
{
    [Table("INVOICE")]
    public class INVOICE
    {
        [Key]
        [Column("InvoiceId")]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("InvoiceNumber")]
        public string InvoiceNumber { get; set; } = null!;

        [Required]
        [Column("BookingId")]
        public int BookingId { get; set; }

        [Column("IssueDate")]
        public DateTime? IssueDate { get; set; } = DateTime.Now;

        [Required]
        [Column("Subtotal", TypeName = "decimal(10, 2)")]
        public decimal Subtotal { get; set; }

        [Column("TaxRate", TypeName = "decimal(5, 2)")]
        public decimal? TaxRate { get; set; } = 10.00m;

        [Required]
        [Column("TaxAmount", TypeName = "decimal(10, 2)")]
        public decimal TaxAmount { get; set; }

        [Required]
        [Column("TotalAmount", TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(500)]
        [Column("Notes")]
        public string? Notes { get; set; }

        [ForeignKey("BookingId")]
        public virtual BOOKING? Booking { get; set; }
    }
}
