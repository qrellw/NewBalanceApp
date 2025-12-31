using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalanceApp.Models
{
    public class TestSample
    {
        [Key]
        public long SampleId { get; set; }

        public int SessionId { get; set; }

        public int Index { get; set; } // Sequence number (0, 1, 2...)
        public double TimestampMs { get; set; } // Time from start in ms

        public double X { get; set; }
        public double Y { get; set; }
        
        public double Force1 { get; set; }
        public double Force2 { get; set; }
        public double Force3 { get; set; }
        public double Force4 { get; set; }

        // Navigation property
        [ForeignKey("SessionId")]
        public TestSession? TestSession { get; set; }
    }
}
