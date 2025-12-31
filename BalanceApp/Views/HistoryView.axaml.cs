using Avalonia.Controls;
using BalanceApp.ViewModels;
using ScottPlot;
using System;
using System.Linq;

namespace BalanceApp.Views;

public partial class HistoryView : UserControl
{
    public HistoryView()
    {
        InitializeComponent();
        
        var plot = HistoryPlot.Plot;
        plot.Title("Biểu Đồ Lịch Sử");
        plot.XLabel("Left - Right (cm)");
        plot.YLabel("Back - Front (cm)");
        plot.Axes.SquareUnits(); // 1:1 Aspect Ratio
        plot.Grid.MajorLineColor = Colors.LightGray.WithOpacity(0.5);
        plot.Add.Line(0, -15, 0, 15);
        plot.Add.Line(-15, 0, 15, 0);
    }

    private HistoryViewModel? _viewModel;

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is HistoryViewModel vm)
        {
            _viewModel = vm;
            _viewModel.SessionSelected += OnSessionSelected;
        }
    }

    private void OnSessionSelected(Models.TestSession session)
    {
        HistoryPlot.Plot.Clear(); // Clear old plottables (except grid/axes config ideally, but ScottPlot 5 Clear removes everything?)
        // ScottPlot 5 Clear() removes plottables. We need to re-add layout if needed, or just remove specific ones.
        // Actually plot.Clear() removes everything. Let's re-setup or just remove traces.
        // Better: plot.Remove(plottable). 
        
        // Simpler: Clear and Re-add basics
        var plot = HistoryPlot.Plot;
        plot.Clear();
        plot.Grid.MajorLineColor = Colors.LightGray.WithOpacity(0.5);
        plot.Add.Line(0, -15, 0, 15);
        plot.Add.Line(-15, 0, 15, 0);

        if (session.TestSamples != null && session.TestSamples.Any())
        {
            var xs = session.TestSamples.Select(s => (double)s.X).ToArray();
            var ys = session.TestSamples.Select(s => (double)s.Y).ToArray();

            var scatter = plot.Add.Scatter(xs, ys);
            scatter.Color = Colors.Black;
            scatter.LineWidth = 1;
            scatter.MarkerSize = 3;
        }

        HistoryPlot.Refresh();
    }
}
