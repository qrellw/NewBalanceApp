using System;
using System.ComponentModel.DataAnnotations;

namespace BalanceApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public required string PasswordHash { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        public string Role { get; set; } = "Doctor"; // Admin, Doctor
    }
}
