using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMS.Models.Entities
{
    [Table("HOTEL_STAFF")]
    public class HOTEL_STAFF
    {
        [Key]
        [Column("StaffId")]
        public int StaffId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("Role")]
        public string Role { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Column("Username")]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(255)]
        [Column("PasswordHash")]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Column("Email")]
        public string Email { get; set; } = null!;

        [StringLength(20)]
        [Column("Phone")]
        public string? Phone { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
