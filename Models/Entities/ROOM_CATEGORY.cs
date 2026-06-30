using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models.Entities
{
    [Table("ROOM_CATEGORY")]
    public class ROOM_CATEGORY
    {
        [Key]
        [Column("CategoryId")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("CategoryName")]
        public string CategoryName { get; set; } = null!;

        [Required]
        [Column("BasePrice", TypeName = "decimal(10, 2)")]
        public decimal BasePrice { get; set; }

        [StringLength(500)]
        [Column("Description")]
        public string? Description { get; set; }

        [Required]
        [Column("MaxOccupancy")]
        public int MaxOccupancy { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<ROOM> Rooms { get; set; } = new List<ROOM>();
    }
}
