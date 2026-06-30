using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models.Entities
{
    [Table("SERVICES")]
    public class SERVICE
    {
        [Key]
        [Column("ServiceId")]
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("ServiceName")]
        public string ServiceName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("ServiceType")]
        public string ServiceType { get; set; } = "Other";

        [Column("Description")]
        public string? Description { get; set; }

        [Required]
        [Column("Price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column("IsAvailable")]
        public bool IsAvailable { get; set; } = true;

        // Navigation property for booking services
        public virtual ICollection<BOOKING_SERVICE>? BookingServices { get; set; }
    }
}
