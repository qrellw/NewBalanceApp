using Avalonia.Controls;
using System;
using System.Linq;
using ScottPlot;
using ScottPlot.Plottables;
using BalanceApp.ViewModels;

namespace BalanceApp.Views;

public partial class MeasurementView : UserControl
{
    public MeasurementView()
    {
        InitializeComponent();
        
        // Setup Plot
        var plot = BalancePlot.Plot;
        plot.Title("Tâm Áp Lực (COP)");
        plot.Axes.SquareUnits(); // 1:1 Aspect Ratio
        plot.XLabel("Left - Right (cm)");
        plot.YLabel("Back - Front (cm)");
        
        // Grid
        plot.Grid.MajorLineColor = Colors.LightGray.WithOpacity(0.5);
        
        // Crosshair
        plot.Add.Line(0, -15, 0, 15);
        plot.Add.Line(-15, 0, 15, 0);

        // 1. History Plot (Faded, background)
        _historyPlot = plot.Add.Scatter(Array.Empty<double>(), Array.Empty<double>());
        _historyPlot.MarkerSize = 3;
        _historyPlot.Color = Colors.Gray.WithOpacity(0.3); // Faded
        _historyPlot.LineWidth = 0; // No lines for history to reduce clutter? Or faint line? Let's use faint line
        _historyPlot.LineWidth = 1;

        // 2. Active Plot (Recent 30 pts, Bold Black)
        _activePlot = plot.Add.Scatter(Array.Empty<double>(), Array.Empty<double>());
        _activePlot.MarkerSize = 5;
        _activePlot.Color = Colors.Black;
        _activePlot.LineWidth = 2; // Connected lines
        
        // 3. Current Head (Big Dot)
        _currentHead = plot.Add.Marker(0, 0);
        _currentHead.Color = Colors.Black;
        _currentHead.Size = 10;
        _currentHead.Shape = MarkerShape.FilledCircle;
    }

    private Scatter? _historyPlot;
    private Scatter? _activePlot;
    private Marker? _currentHead;
    
    private readonly System.Collections.Generic.List<(double X, double Y)> _historyPoints = new();
    private readonly System.Collections.Generic.Queue<(double X, double Y)> _activeQueue = new();
    private const int MaxActiveLength = 30;

    private MeasurementViewModel? _viewModel;

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MeasurementViewModel vm)
        {
            _viewModel = vm;
            _viewModel.UpdateGraphAction += OnGraphUpdate;
        }
    }

    private void OnGraphUpdate(Services.Sensor.SensorData data)
    {
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            // Reset signal
            if (data == null)
            {
                _historyPoints.Clear();
                _activeQueue.Clear();
                UpdatePlots();
                return;
            }

            if (_activePlot == null || _historyPlot == null || _currentHead == null) return;

            // Update Head
            _currentHead.X = data.X;
            _currentHead.Y = data.Y;

            // Logic: Add to Active Queue
            _activeQueue.Enqueue((data.X, data.Y));

            // If Active Queue full, move oldest to History
            if (_activeQueue.Count > MaxActiveLength)
            {
                var old = _activeQueue.Dequeue();
                _historyPoints.Add(old);
            }

            // Redraw
            UpdatePlots();
        });
    }

    private void UpdatePlots()
    {
        if (_activePlot != null) BalancePlot.Plot.Remove(_activePlot);
        if (_historyPlot != null) BalancePlot.Plot.Remove(_historyPlot);

        // Update History
        if (_historyPoints.Any())
        {
            var hXs = _historyPoints.Select(p => p.X).ToArray();
            var hYs = _historyPoints.Select(p => p.Y).ToArray();
            _historyPlot = BalancePlot.Plot.Add.Scatter(hXs, hYs);
            _historyPlot.MarkerSize = 3;
            _historyPlot.Color = Colors.Gray.WithOpacity(0.3);
            _historyPlot.LineWidth = 1;
        }

        // Update Active
        if (_activeQueue.Any())
        {
            var activeArr = _activeQueue.ToArray();
            var aXs = activeArr.Select(p => p.X).ToArray();
            var aYs = activeArr.Select(p => p.Y).ToArray();
            _activePlot = BalancePlot.Plot.Add.Scatter(aXs, aYs);
            _activePlot.MarkerSize = 5;
            _activePlot.Color = Colors.Black;
            _activePlot.LineWidth = 2;
        }
        
        // Ensure Head is on top
        if (_currentHead != null)
        {
            BalancePlot.Plot.Remove(_currentHead);
            _currentHead = BalancePlot.Plot.Add.Marker(_currentHead.X, _currentHead.Y);
            _currentHead.Color = Colors.Black;
            _currentHead.Size = 10;
            _currentHead.Shape = MarkerShape.FilledCircle;
        }
        
        BalancePlot.Refresh();
    }
}
