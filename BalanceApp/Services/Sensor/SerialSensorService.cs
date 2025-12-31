using System;
using System.IO.Ports;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace BalanceApp.Services.Sensor;

public class SerialSensorService : ISensorService
{
    private SerialPort? _serialPort;
    private Thread? _readThread;
    private bool _keepReading;

    public event Action<SensorData>? DataReceived;
    public event Action<string>? StatusChanged;

    public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

    public void Connect(string portName)
    {
        Disconnect();

        try
        {
            _serialPort = new SerialPort(portName, 115200);
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPort.Open();

            _keepReading = true;
            _readThread = new Thread(ReadPort);
            _readThread.Start();

            StatusChanged?.Invoke($"Đã kết nối {portName}");
        }
        catch (Exception ex)
        {
            StatusChanged?.Invoke($"Lỗi kết nối: {ex.Message}");
        }
    }

    public void Disconnect()
    {
        _keepReading = false;
        if (_readThread != null && _readThread.IsAlive)
        {
            try { _readThread.Join(500); } catch { }
        }

        if (_serialPort != null)
        {
            try 
            { 
                if (_serialPort.IsOpen) _serialPort.Close(); 
                _serialPort.Dispose(); 
            } 
            catch { }
            _serialPort = null;
        }

        StatusChanged?.Invoke("Đã ngắt kết nối");
    }

    public async Task<bool> ScanAndConnectAsync()
    {
        Disconnect();
        StatusChanged?.Invoke("Đang quét thiết bị...");

        var ports = SerialPort.GetPortNames();
        foreach (var port in ports)
        {
            try
            {
                StatusChanged?.Invoke($"Đang thử {port}...");
                _serialPort = new SerialPort(port, 115200);
                _serialPort.ReadTimeout = 1000; // 1s timeout
                _serialPort.Open();

                // Listen for 1.5 seconds to see if valid JSON comes
                // Simple validation: Read 1 lines and check signature
                _serialPort.DiscardInBuffer();
                
                string? line = null;
                try { line = _serialPort.ReadLine(); } catch { } 
                // Try second line to avoid partial
                try { line = _serialPort.ReadLine(); } catch { }

                if (!string.IsNullOrEmpty(line) && line.Contains("\"ts\":"))
                {
                    // FOUND IT!
                    StatusChanged?.Invoke($"Đã tìm thấy ESP32 tại {port}");
                    
                    _keepReading = true;
                    _readThread = new Thread(ReadPort);
                    _readThread.Start();
                    return true;
                }
                
                _serialPort.Close();
                _serialPort.Dispose();
                _serialPort = null;
            }
            catch 
            {
                // Ignore error and try next port
                if (_serialPort != null && _serialPort.IsOpen) _serialPort.Close();
                _serialPort = null;
            }
        }

        StatusChanged?.Invoke("Không tìm thấy thiết bị nào");
        return false;
    }

    public void StartReading() { /* Already reading in loop */ }
    public void StopReading() { /* Handled by Disconnect or logic flag */ }

    private void ReadPort()
    {
        while (_keepReading)
        {
            try
            {
                if (_serialPort == null || !_serialPort.IsOpen) break;

                string line = _serialPort.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                ParseLine(line);
            }
            catch (TimeoutException) { }
            catch (Exception ex)
            {
                // StatusChanged?.Invoke($"Lỗi đọc: {ex.Message}");
            }
        }
    }

    private void ParseLine(string line)
    {
        try
        {
            // Expected format: {"ts":123,"x":0.1,"y":0.2,"f1":10...}
            int startIndex = line.IndexOf('{');
            int endIndex = line.LastIndexOf('}');

            if (startIndex != -1 && endIndex != -1)
            {
                string json = line.Substring(startIndex, endIndex - startIndex + 1);
                var data = JsonSerializer.Deserialize<SensorDataRaw>(json);
                
                if (data != null)
                {
                    var sensorData = new SensorData
                    {
                        Timestamp = data.ts,
                        X = data.x,
                        Y = data.y,
                        Force1 = data.f1,
                        Force2 = data.f2,
                        Force3 = data.f3,
                        Force4 = data.f4
                    };
                    DataReceived?.Invoke(sensorData);
                }
            }
        }
        catch { /* Ignore parse errors */ }
    }

    private class SensorDataRaw
    {
        public long ts { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float f1 { get; set; }
        public float f2 { get; set; }
        public float f3 { get; set; }
        public float f4 { get; set; }
    }
}
