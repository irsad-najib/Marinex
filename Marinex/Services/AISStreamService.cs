using System;
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

        public event EventHandler<ShipPosition> OnShipPositionReceived;
        public event EventHandler<bool> OnConnectionStatusChanged;
        public event EventHandler<string> OnError;

        public AISStreamService(string apiKey)
        {
            _apiKey = apiKey;
            _ws = new ClientWebSocket();
        }

        public async Task StartStreamAsync()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                
                // Connect to WebSocket
                await _ws.ConnectAsync(
                    new Uri("wss://stream.aisstream.io/v0/stream"), 
                    _cancellationTokenSource.Token);

                _isConnected = true;
                OnConnectionStatusChanged?.Invoke(this, true);

                // Subscribe to AIS data
                await SubscribeToAISDataAsync();

                // Start receiving loop
                _ = ReceiveLoopAsync();
            }
            catch (Exception ex)
            {
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
                BoundingBoxes = new double[][][]
                {
                    new double[][]
                    {
                        new double[] { -90, -180 },  // Southwest corner
                        new double[] { 90, 180 }     // Northeast corner
                    }
                },
                FilterMessageTypes = new string[] { "PositionReport" }
            };

            string json = JsonSerializer.Serialize(subscribeMsg);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            
            await _ws.SendAsync(
                new ArraySegment<byte>(buffer), 
                WebSocketMessageType.Text, 
                true, 
                _cancellationTokenSource.Token);
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[8192];
            
            try
            {
                while (_ws.State == WebSocketState.Open && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var result = await _ws.ReceiveAsync(
                        new ArraySegment<byte>(buffer), 
                        _cancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        ProcessMessage(message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _ws.CloseAsync(
                            WebSocketCloseStatus.NormalClosure, 
                            "Closing", 
                            _cancellationTokenSource.Token);
                        
                        _isConnected = false;
                        OnConnectionStatusChanged?.Invoke(this, false);
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
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var aisMessage = JsonSerializer.Deserialize<AISMessage>(message, options);
                
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

                    // Validate coordinates
                    if (IsValidCoordinate(shipPosition.Latitude, shipPosition.Longitude))
                    {
                        OnShipPositionReceived?.Invoke(this, shipPosition);
                    }
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, $"Parse error: {ex.Message}");
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
