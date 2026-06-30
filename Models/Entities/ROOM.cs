using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models.Entities
{
    [Table("ROOM")]
    public class ROOM
    {
        [Key]
        [Column("RoomId")]
        public int RoomId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("RoomNumber")]
        public string RoomNumber { get; set; } = null!;

        [Required]
        [Column("CategoryId")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Status")]
        public string Status { get; set; } = "Available";

        [StringLength(10)]
        [Column("Floor")]
        public string? Floor { get; set; }

        [StringLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        [ForeignKey("CategoryId")]
        public virtual ROOM_CATEGORY? Category { get; set; }

        public virtual ICollection<BOOKING> Bookings { get; set; } = new List<BOOKING>();
    }
}
