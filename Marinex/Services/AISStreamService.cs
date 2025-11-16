using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Marinex.Models;

namespace Marinex.Services
{
    public class AISStreamService
    {
        private ClientWebSocket _ws;
        private readonly string _apiKey;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isConnected = false;
        private readonly string _logFilePath;

        public event EventHandler<ShipPosition> OnShipPositionReceived;
        public event EventHandler<bool> OnConnectionStatusChanged;
        public event EventHandler<string> OnError;

        public AISStreamService(string apiKey)
        {
            _apiKey = apiKey;
            _ws = new ClientWebSocket();
            
            // Create log file in user's Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _logFilePath = Path.Combine(documentsPath, "Marinex_AIS_Log.txt");
            
            // Clear old log and write header
            File.WriteAllText(_logFilePath, $"=== MARINEX AIS STREAM LOG ===\n");
            File.AppendAllText(_logFilePath, $"Started: {DateTime.Now}\n");
            File.AppendAllText(_logFilePath, $"Log file: {_logFilePath}\n\n");
            
            WriteLog($"AISStreamService initialized with API key: {apiKey.Substring(0, 10)}...");
        }

        public async Task StartStreamAsync()
        {
            try
            {
                WriteLog("Starting WebSocket connection...");
                _cancellationTokenSource = new CancellationTokenSource();
                
                // Connect to WebSocket
                await _ws.ConnectAsync(
                    new Uri("wss://stream.aisstream.io/v0/stream"), 
                    _cancellationTokenSource.Token);

                WriteLog($"WebSocket connected! State: {_ws.State}");
                _isConnected = true;
                OnConnectionStatusChanged?.Invoke(this, true);

                // Subscribe to AIS data
                await SubscribeToAISDataAsync();

                // Start receiving loop
                WriteLog("Starting receive loop...");
                _ = ReceiveLoopAsync();
            }
            catch (Exception ex)
            {
                WriteLog($"ERROR in StartStreamAsync: {ex.Message}\n{ex.StackTrace}");
                _isConnected = false;
                OnConnectionStatusChanged?.Invoke(this, false);
                OnError?.Invoke(this, $"Connection failed: {ex.Message}");
                throw;
            }
        }

        private async Task SubscribeToAISDataAsync()
        {
            var subscribeMsg = new
            {
                APIKey = _apiKey,
                // ShipMMSI = new int[] { },  // Empty array to receive all ships in bounding box
                BoundingBoxes = new double[][][]
                {
                    new double[][]
                    {
                        new double[] { 1.0, 103.0 },  // Southwest corner (Singapore)
                        new double[] { 2.0, 104.0 }     // Northeast corner (Singapore)
                    }
                },
                // FilterMessageTypes = new string[] { "PositionReport" }
            };

            string json = JsonSerializer.Serialize(subscribeMsg);
            WriteLog($"Sending subscription: {json}");
            
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            
            await _ws.SendAsync(
                new ArraySegment<byte>(buffer), 
                WebSocketMessageType.Text, 
                true, 
                _cancellationTokenSource.Token);
                
            WriteLog($"Subscription sent successfully!");
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[8192];
            
            try
            {
                while (_ws.State == WebSocketState.Open && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    using var messageStream = new System.IO.MemoryStream();
                    WebSocketReceiveResult result;

                    do
                    {
                        result = await _ws.ReceiveAsync(
                            new ArraySegment<byte>(buffer), 
                            _cancellationTokenSource.Token);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _ws.CloseAsync(
                                WebSocketCloseStatus.NormalClosure, 
                                "Closing", 
                                _cancellationTokenSource.Token);

                            _isConnected = false;
                            OnConnectionStatusChanged?.Invoke(this, false);
                            return;
                        }

                        messageStream.Write(buffer, 0, result.Count);
                    }
                    while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        messageStream.Seek(0, System.IO.SeekOrigin.Begin);
                        string message = Encoding.UTF8.GetString(messageStream.ToArray());
                        
                        // Debug: Log that we received a message
                        Console.WriteLine($"[AIS] Received message: {message.Substring(0, Math.Min(100, message.Length))}...");
                        
                        ProcessMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                _isConnected = false;
                OnConnectionStatusChanged?.Invoke(this, false);
                OnError?.Invoke(this, $"Receive error: {ex.Message}");
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {
                Console.WriteLine($"[AIS] Processing message...");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var aisMessage = JsonSerializer.Deserialize<AISMessage>(message, options);
                
                Console.WriteLine($"[AIS] Deserialized: Message={aisMessage?.Message != null}, PositionReport={aisMessage?.Message?.PositionReport != null}");
                
                if (aisMessage?.Message?.PositionReport != null)
                {
                    var posReport = aisMessage.Message.PositionReport;
                    var metaData = aisMessage.MetaData;

                    var shipPosition = new ShipPosition
                    {
                        Mmsi = posReport.UserID?.ToString() ?? "Unknown",
                        ShipName = metaData?.ShipName ?? "Unknown",
                        Latitude = posReport.Latitude,
                        Longitude = posReport.Longitude,
                        Speed = posReport.Sog ?? 0,  // Speed Over Ground
                        Course = posReport.Cog ?? 0, // Course Over Ground
                        Heading = posReport.TrueHeading ?? 0,
                        ShipType = metaData?.ShipType ?? "Unknown",
                        Destination = metaData?.Destination ?? "Unknown",
                        LastUpdate = DateTime.Now
                    };

                    Console.WriteLine($"[AIS] Ship: {shipPosition.ShipName} at ({shipPosition.Latitude}, {shipPosition.Longitude})");

                    // Validate coordinates
                    if (IsValidCoordinate(shipPosition.Latitude, shipPosition.Longitude))
                    {
                        Console.WriteLine($"[AIS] ✓ Valid coordinates, invoking event...");
                        OnShipPositionReceived?.Invoke(this, shipPosition);
                    }
                    else
                    {
                        Console.WriteLine($"[AIS] ✗ Invalid coordinates: ({shipPosition.Latitude}, {shipPosition.Longitude})");
                    }
                }
                else
                {
                    Console.WriteLine($"[AIS] ✗ No PositionReport in message");
                }
            }
            catch (Exception ex)
            {
                WriteLog($"ERROR in ProcessMessage: {ex.Message}\n{ex.StackTrace}");
                OnError?.Invoke(this, $"Parse error: {ex.Message}");
            }
        }

        private void WriteLog(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n";
                File.AppendAllText(_logFilePath, logEntry);
                Console.WriteLine($"[AIS] {message}");
            }
            catch
            {
                // Ignore logging errors
            }
        }

        private bool IsValidCoordinate(double latitude, double longitude)
        {
            return latitude >= -90 && latitude <= 90 && 
                   longitude >= -180 && longitude <= 180 &&
                   latitude != 0 && longitude != 0;
        }

        public async Task StopStreamAsync()
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                
                if (_ws?.State == WebSocketState.Open)
                {
                    await _ws.CloseAsync(
                        WebSocketCloseStatus.NormalClosure, 
                        "Client closing", 
                        CancellationToken.None);
                }

                _isConnected = false;
                OnConnectionStatusChanged?.Invoke(this, false);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Close error: {ex.Message}");
            }
            finally
            {
                _ws?.Dispose();
                _ws = new ClientWebSocket();
            }
        }

        // AIS Message structure for deserialization
        private class AISMessage
        {
            [JsonPropertyName("Message")]
            public MessageData Message { get; set; }

            [JsonPropertyName("MetaData")]
            public MetaData MetaData { get; set; }
        }

        private class MessageData
        {
            [JsonPropertyName("PositionReport")]
            public PositionReport PositionReport { get; set; }
        }

        private class PositionReport
        {
            [JsonPropertyName("UserID")]
            public long? UserID { get; set; }

            [JsonPropertyName("Latitude")]
            public double Latitude { get; set; }

            [JsonPropertyName("Longitude")]
            public double Longitude { get; set; }

            [JsonPropertyName("Sog")]
            public double? Sog { get; set; }

            [JsonPropertyName("Cog")]
            public double? Cog { get; set; }

            [JsonPropertyName("TrueHeading")]
            public double? TrueHeading { get; set; }
        }

        private class MetaData
        {
            [JsonPropertyName("ShipName")]
            public string ShipName { get; set; }

            [JsonPropertyName("ShipType")]
            public string ShipType { get; set; }

            [JsonPropertyName("Destination")]
            public string Destination { get; set; }
        }
    }
}
