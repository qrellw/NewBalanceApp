using Avalonia.Controls;
using System;
using System.Linq;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.DataSources;
using BalanceApp.ViewModels;
using System.Collections.Generic;
using Avalonia.Threading;

namespace BalanceApp.Views;

public partial class MeasurementView : UserControl
{
    private Scatter _historyPlot;
    private Scatter _activePlot;
    private Marker _currentHead;
    
    // Data buffers
    // ScottPlot sẽ "nhìn" trực tiếp vào 4 cái List này để vẽ
    private readonly List<double> _historyX = new();
    private readonly List<double> _historyY = new();
    
    private readonly List<double> _activeX = new();
    private readonly List<double> _activeY = new();
    
    private MeasurementViewModel? _viewModel;
    private readonly object _lock = new();

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

        // --- SỬA ĐOẠN 1: LIÊN KẾT LIST VÀO BIỂU ĐỒ ---

        // 1. History Plot (Faded)
        // Truyền trực tiếp biến _historyX, _historyY vào hàm Add.Scatter
        _historyPlot = plot.Add.Scatter(_historyX, _historyY);
        _historyPlot.MarkerSize = 3;
        _historyPlot.Color = Colors.Gray.WithOpacity(0.3);
        _historyPlot.LineWidth = 1;

        // 2. Active Plot (Recent)
        // Truyền trực tiếp biến _activeX, _activeY vào hàm Add.Scatter
        _activePlot = plot.Add.Scatter(_activeX, _activeY);
        _activePlot.MarkerSize = 5;
        _activePlot.Color = Colors.Black;
        _activePlot.LineWidth = 2; // Connected lines
        
        // 3. Current Head (Big Dot)
        _currentHead = plot.Add.Marker(0, 0);
        _currentHead.Color = Colors.Black;
        _currentHead.Size = 10;
        _currentHead.Shape = MarkerShape.FilledCircle;
    }

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
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            lock (_lock)
            {
                // Reset signal
                if (data == null)
                {
                    _historyX.Clear();
                    _historyY.Clear();
                    _activeX.Clear();
                    _activeY.Clear();
                    
                    // --- SỬA ĐOẠN 2: BỎ LỆNH GÁN .DATA ---
                    // Chỉ cần Refresh là nó tự nhận diện List rỗng
                    BalancePlot.Refresh();
                    return;
                }

                // Update Head
                _currentHead.X = data.X;
                _currentHead.Y = data.Y;

                // Add to Active buffer
                _activeX.Add(data.X);
                _activeY.Add(data.Y);

                // If Active full, move oldest to History
                if (_activeX.Count > 30) // MaxActiveLength
                {
                    double oldX = _activeX[0];
                    double oldY = _activeY[0];
                    
                    _activeX.RemoveAt(0);
                    _activeY.RemoveAt(0);
                    
                    _historyX.Add(oldX);
                    _historyY.Add(oldY);
                }

                // --- SỬA ĐOẠN 3: BỎ LỆNH GÁN .DATA ---
                // Không cần làm gì cả vì List đã thay đổi ở trên

                // Refresh để vẽ lại theo dữ liệu mới trong List
                BalancePlot.Refresh();
            }
        });
    }
}