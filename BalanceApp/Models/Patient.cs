using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BalanceApp.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string FullName { get; set; }

        [MaxLength(50)]
        public string? MedicalId { get; set; } // Mã bệnh án

        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; } // Nam, Nu

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(50)]
        public string? Ethnicity { get; set; } // Dân tộc

        public double? Height { get; set; } // cm
        public double? Weight { get; set; } // kg

        [MaxLength(100)]
        public string? Job { get; set; } // Nghề nghiệp

        public string? MedicalHistory { get; set; } // Tiền sử bệnh lý

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<TestSession> TestSessions { get; set; } = new List<TestSession>();
    }
}
