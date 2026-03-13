using System;
using System.Threading.Tasks;

namespace BalanceApp.Services.Sensor;

public class SensorData
{
    public long Timestamp { get; set; } // Sequence or time
    public float X { get; set; }
    public float Y { get; set; }
    public float Force1 { get; set; }
    public float Force2 { get; set; }
    public float Force3 { get; set; }
    public float Force4 { get; set; }
    
    // Calculated
    public float TotalWeight => Force1 + Force2 + Force3 + Force4;
}

public interface ISensorService
{
    bool IsConnected { get; }
    bool IsSimulationMode { get; } // New property

    event Action<SensorData>? DataReceived;
    event Action<string>? StatusChanged;

    void ToggleSimulation(bool enable); // New method

    void Connect(string connectionString); // COM port or "WIFI"
    Task<bool> ScanAndConnectAsync(); // Auto-detect
    void Disconnect();
    void StartReading();
    void StopReading();
}
