using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models.Entities
{
    [Table("CUSTOMER")]
    public class CUSTOMER
    {
        [Key]
        [Column("CustomerId")]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Column("Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [Column("Phone")]
        public string Phone { get; set; } = null!;

        [StringLength(500)]
        [Column("Address")]
        public string? Address { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Username")]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(255)]
        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<BOOKING> Bookings { get; set; } = new List<BOOKING>();
    }
}
