using BalanceApp.Models;
using BalanceApp.Services;
using BalanceApp.Services.Sensor;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BalanceApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentView;

    [ObservableProperty]
    private bool _isPaneOpen = true;

    public PatientViewModel PatientVM { get; }
    public MeasurementViewModel MeasurementVM { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPatientSelected))]
    [NotifyPropertyChangedFor(nameof(SelectedPatientName))]
    private Patient? _selectedPatient;

    public bool IsPatientSelected => SelectedPatient != null;
    public string SelectedPatientName => SelectedPatient != null 
        ? $"{SelectedPatient.FullName} - {SelectedPatient.MedicalId}" 
        : "Chưa chọn bệnh nhân";

    public ISensorService SensorService { get; }
    public TestSessionService SessionService { get; }

    public MainViewModel(ISensorService sensorService, TestSessionService sessionService)
    {
        SensorService = sensorService;
        SessionService = sessionService;

        // Inject dependencies or pass 'this' to child VMs for context sharing
        PatientVM = new PatientViewModel(this); 
        MeasurementVM = new MeasurementViewModel(this);

        _currentView = PatientVM; 
    }

    [RelayCommand]
    private void GoToPatients()
    {
        CurrentView = PatientVM;
    }

    [RelayCommand]
    public void GoToMeasurement()
    {
        CurrentView = MeasurementVM;
    }
    
    [RelayCommand]
    private void TogglePane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    public void GoToHistory(Patient patient)
    {
        var historyVM = new HistoryViewModel(this, patient);
        CurrentView = historyVM;
    }
}
