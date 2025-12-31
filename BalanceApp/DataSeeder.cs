using System;
using System.Linq;
using BalanceApp.Models;
using BalanceApp.Services;

namespace BalanceApp;

public static class DataSeeder
{
    public static void Seed()
    {
        using var context = new BalanceDbContext();
        context.Database.EnsureCreated();

        if (!context.Patients.Any())
        {
            context.Patients.AddRange(
                new Patient { FullName = "Nguyễn Văn A", DateOfBirth = new DateTime(1980, 1, 1), Gender = "Nam", PhoneNumber = "0901234567", Address = "Hà Nội" },
                new Patient { FullName = "Trần Thị B", DateOfBirth = new DateTime(1990, 5, 15), Gender = "Nữ", PhoneNumber = "0912345678", Address = "Hồ Chí Minh" },
                new Patient { FullName = "Lê Văn C", DateOfBirth = new DateTime(1975, 12, 20), Gender = "Nam", PhoneNumber = "0987654321", Address = "Đà Nẵng" }
            );
            context.SaveChanges();
        }
    }
}
