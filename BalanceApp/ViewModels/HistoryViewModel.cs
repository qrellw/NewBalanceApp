using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BalanceApp.Models;
using BalanceApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScottPlot;
using System;
using System.Collections.Generic;

namespace BalanceApp.ViewModels;

public partial class HistoryViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private readonly TestSessionService _sessionService;
    private readonly Patient _patient;

    [ObservableProperty]
    private string _patientName;

    [ObservableProperty]
    private ObservableCollection<TestSession> _sessions = new();

    [ObservableProperty]
    private TestSession? _selectedSession;

    public HistoryViewModel(MainViewModel mainViewModel, Patient patient)
    {
        _mainViewModel = mainViewModel;
        _patient = patient;
        _sessionService = _mainViewModel.SessionService;
        _patientName = patient.FullName;

        LoadHistoryCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadHistory()
    {
        var list = await _sessionService.GetSessionsByPatientIdAsync(_patient.PatientId);
        Sessions = new ObservableCollection<TestSession>(list);
    }

    [RelayCommand]
    private void GoBack()
    {
        _mainViewModel.CurrentView = _mainViewModel.PatientVM;
    }

    [RelayCommand]
    private async Task DeleteSession(TestSession? session)
    {
        if (session == null) return;

        try
        {
            await _sessionService.DeleteSessionAsync(session.SessionId);
            
            // UI Update
            Sessions.Remove(session);
            
            // If deleted session was selected, clear selection
            if (SelectedSession == session)
            {
                SelectedSession = null;
                SessionSelected?.Invoke(null!);
            }
        }
        catch (Exception ex)
        {
             // Ideally show error dialog
             Console.WriteLine("Error deleting session: " + ex.Message);
        }
    }

    // Event to notify View to redraw graph
    public event Action<TestSession>? SessionSelected;

    partial void OnSelectedSessionChanged(TestSession? value)
    {
        if (value != null)
        {
            SessionSelected?.Invoke(value);
        }
    }
}
