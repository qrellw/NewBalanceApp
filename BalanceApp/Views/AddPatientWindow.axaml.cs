using Avalonia.Controls;
using Avalonia.Interactivity;

namespace BalanceApp.Views;

public partial class AddPatientWindow : Window
{
    public AddPatientWindow()
    {
        InitializeComponent();
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
