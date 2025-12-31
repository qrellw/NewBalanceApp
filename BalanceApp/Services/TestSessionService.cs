using System;
using System.Threading.Tasks;
using BalanceApp.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BalanceApp.Services;

public class TestSessionService
{
    private readonly BalanceDbContext _context;

    public TestSessionService()
    {
        _context = new BalanceDbContext();
    }

    public async Task<int> SaveSessionAsync(TestSession session)
    {
        try
        {
            // Verify patient exists attached or fetch
            var patient = await _context.Patients.FindAsync(session.PatientId);
            if (patient == null) throw new Exception("Patient not found");

            session.Patient = patient; // key up
            
            _context.TestSessions.Add(session);
            await _context.SaveChangesAsync();
            return session.SessionId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving session: {ex.Message}");
            throw;
        }
    }
    public async Task<List<TestSession>> GetSessionsByPatientIdAsync(int patientId)
    {
        return await _context.TestSessions
            .Include(s => s.TestSamples)
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.TestDate)
            .ToListAsync();
    }
}
