using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BalanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BalanceApp.Services;

public class PatientService
{
    private readonly BalanceDbContext _context;

    public PatientService()
    {
        _context = new BalanceDbContext();
    }

    public async Task<List<Patient>> GetAllPatientsAsync()
    {
        return await _context.Patients.AsNoTracking().OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Patient> AddPatientAsync(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task DeletePatientAsync(int id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient != null)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
    }

    // Add search/filter methods later if needed
}
