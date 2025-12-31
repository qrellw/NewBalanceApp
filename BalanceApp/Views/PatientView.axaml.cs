using Avalonia.Controls;

namespace BalanceApp.Views;

public partial class PatientView : UserControl
{
    public PatientView()
    {
        InitializeComponent();
    }

    private void OpenAddPatient_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = new AddPatientWindow();
        window.DataContext = this.DataContext; // Share same ViewModel
        
        // Find main window as parent
        var topLevel = Avalonia.Controls.TopLevel.GetTopLevel(this) as Window;
        if (topLevel != null)
        {
            window.ShowDialog(topLevel);
        }
    }
}
