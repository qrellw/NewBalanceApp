using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalanceApp.Models
{
    public class TestSession
    {
        [Key]
        public int SessionId { get; set; }

        public int PatientId { get; set; }

        public DateTime TestDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Metrics (Calculated after test)
        public double MeanX { get; set; }
        public double MeanY { get; set; }
        public double BMI { get; set; }

        // Navigation properties
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        public ICollection<TestSample> TestSamples { get; set; } = new List<TestSample>();
    }
}
