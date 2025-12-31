using BalanceApp.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BalanceApp.Services
{
    public class BalanceDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<TestSession> TestSessions { get; set; }
        public DbSet<TestSample> TestSamples { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Use SQL Server Express instance as requested
            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=BalanceAppDB_v2;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TestSample to act as high-volume data
            modelBuilder.Entity<TestSample>()
                .HasIndex(s => s.SessionId); // Optimize query by Session
        }
    }
}
