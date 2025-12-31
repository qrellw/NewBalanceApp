using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BalanceApp.Models;
using BalanceApp.Services;
using System.Collections.Generic;
using System;
using System.Linq; // Added for filtering
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BalanceApp.ViewModels;

public partial class PatientViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly PatientService _patientService;

    [ObservableProperty]
    private string _title = "Quản Lý Bệnh Nhân";

    [ObservableProperty]
    private ObservableCollection<Patient> _patients = new();
    
    // Cache for filtering
    private List<Patient> _allPatients = new();
    
    [ObservableProperty]
    private string _errorMessage = "";
    
    [ObservableProperty]
    private string _searchText = "";
    
    partial void OnSearchTextChanged(string value)
    {
        FilterPatients();
    }
    
    private void FilterPatients()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            Patients = new ObservableCollection<Patient>(_allPatients);
        }
        else
        {
            var lower = SearchText.ToLower();
            var filtered = _allPatients.Where(p => 
                p.FullName.ToLower().Contains(lower) || 
                (p.PhoneNumber != null && p.PhoneNumber.Contains(lower)) ||
                (p.MedicalId != null && p.MedicalId.ToLower().Contains(lower))
            ).ToList();
            Patients = new ObservableCollection<Patient>(filtered);
        }
        Title = $"Quản Lý Bệnh Nhân (SL: {Patients.Count})";
    }

    [ObservableProperty]
    private Patient? _selectedPatientInGrid;
    
    // Quick Add Fields
    [ObservableProperty]
    private string _newPatientName = "";
    
    [ObservableProperty]
    private string _newPatientPhone = "";

    [ObservableProperty]
    private string _newPatientAddress = "";

    // -- Detailed Address Fields --
    [ObservableProperty]
    private IEnumerable<string> _provinces = LookupService.Provinces.Keys.ToList();

    [ObservableProperty]
    private string? _selectedProvince;

    [ObservableProperty]
    private IEnumerable<string> _districts = new List<string>();

    [ObservableProperty]
    private string? _selectedDistrict;

    [ObservableProperty]
    private IEnumerable<string> _wards = new List<string>();

    [ObservableProperty]
    private string? _selectedWard;
    
    // -- Ethnicity --
    [ObservableProperty]
    private IEnumerable<string> _ethnicities = LookupService.Ethnicities;

    [ObservableProperty]
    private string? _selectedEthnicity; // Replaces NewPatientEthnicity direct string somewhat

    partial void OnSelectedProvinceChanged(string? value)
    {
        if (value != null && LookupService.Provinces.ContainsKey(value))
        {
            Districts = LookupService.Provinces[value];
            SelectedDistrict = null; // Reset child
            Wards = new List<string>(); // Reset grand-child
        }
    }

    partial void OnSelectedDistrictChanged(string? value)
    {
        if (value != null)
        {
            Wards = LookupService.GetWards(value);
            SelectedWard = null;
        }
    }

    [ObservableProperty]
    private string _newPatientMedicalId = "";

    [ObservableProperty]
    private string _newPatientEthnicity = "";

    [ObservableProperty]
    private string _newPatientJob = "";

    [ObservableProperty]
    private string _newPatientHistory = "";

    [ObservableProperty]
    private string _newPatientHeight = ""; // Bind as string to handle empty/parsing

    [ObservableProperty]
    private string _newPatientWeight = ""; // Bind as string

    [ObservableProperty]
    private DateTimeOffset _newPatientDob = DateTimeOffset.Now.AddYears(-30);

    [ObservableProperty]
    private string _newPatientGender = "Nam";
    
    public List<string> GenderOptions { get; } = new() { "Nam", "Nữ" };

    [ObservableProperty]
    private bool _isAdding;
    
    [ObservableProperty]
    private bool _isAddFormVisible;

    public PatientViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _patientService = new PatientService();
        
        // Initial load
        LoadPatientsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadPatients()
    {
        try 
        {
            ErrorMessage = "";
            Console.WriteLine("DEBUG: Starting LoadPatients...");
            var list = await _patientService.GetAllPatientsAsync();
            Console.WriteLine($"DEBUG: Fetched {list.Count} patients from DB.");
            _allPatients = list;
            FilterPatients(); // Populate Patients
            Console.WriteLine($"DEBUG: Patients collection count: {Patients.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG: Error in LoadPatients: {ex.Message}");
            ErrorMessage = $"Lỗi tải dữ liệu: {ex.Message}";
            
            // Fallback for UI testing
            Patients.Clear();
            Patients.Add(new Patient 
            { 
                FullName = "TEST USER (Check Connection)", 
                DateOfBirth = DateTime.Now,
                Gender = "N/A",
                PhoneNumber = "0000"
            });
        }
    }

    // Command when user clicks "Select" button or double clicks row
    [RelayCommand]
    private void SelectPatient(Patient? patient)
    {
        if (patient == null) return;
        
        _mainViewModel.SelectedPatient = patient;
        
        _mainViewModel.SelectedPatient = patient;
        _mainViewModel.GoToMeasurement();
    }

    [RelayCommand]
    private void ToggleAddForm()
    {
        IsAddFormVisible = !IsAddFormVisible;
    }

    // Called when Grid selection changes
    partial void OnSelectedPatientInGridChanged(Patient? value)
    {
        // Optional: Auto-select context on simple row click?
        // Or require explicit "Select" button?
        // Let's keep context strictly on explicit action or double click.
        // Let's keep context strictly on explicit action or double click.
    }

    [RelayCommand]
    private async Task AddPatient(Avalonia.Controls.Window? window)
    {
        if (string.IsNullOrWhiteSpace(NewPatientName)) return;

        IsAdding = true;
        try
        {
            double.TryParse(NewPatientHeight, out double h);
            double.TryParse(NewPatientWeight, out double w);

            // Construct Full Address
            string fullAddr = NewPatientAddress;
            if (!string.IsNullOrEmpty(SelectedWard)) fullAddr += $", {SelectedWard}";
            if (!string.IsNullOrEmpty(SelectedDistrict)) fullAddr += $", {SelectedDistrict}";
            if (!string.IsNullOrEmpty(SelectedProvince)) fullAddr += $", {SelectedProvince}";

            var newPatient = new Patient
            {
                FullName = NewPatientName,
                MedicalId = NewPatientMedicalId,
                PhoneNumber = NewPatientPhone,
                DateOfBirth = NewPatientDob.Date, 
                Gender = NewPatientGender,
                Address = fullAddr.Trim(',', ' '),
                Ethnicity = SelectedEthnicity ?? NewPatientEthnicity, // Handle dropdown or text? Use Dropdown primarily
                Job = NewPatientJob,
                MedicalHistory = NewPatientHistory,
                Height = h > 0 ? h : null,
                Weight = w > 0 ? w : null
            };

            await _patientService.AddPatientAsync(newPatient);

            // Refresh list
            await LoadPatients();
            
            // Close window if provided
            window?.Close();

            // Clear input
            NewPatientName = "";
            NewPatientPhone = "";
            NewPatientMedicalId = "";
            NewPatientAddress = "";
            NewPatientEthnicity = "";
            NewPatientJob = "";
            NewPatientHistory = "";
            NewPatientHeight = "";
            NewPatientWeight = "";
        }
        finally
        {
            IsAdding = false;
        }
    }

    [RelayCommand]
    private async Task DeletePatient(Patient? patient)
    {
        if (patient == null) return;

        try
        {
            await _patientService.DeletePatientAsync(patient.PatientId);
            await LoadPatients(); // Refresh list
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi xóa bệnh nhân: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ViewHistory(Patient? patient)
    {
        if (patient == null) return;
        _mainViewModel.GoToHistory(patient);
    }
}
