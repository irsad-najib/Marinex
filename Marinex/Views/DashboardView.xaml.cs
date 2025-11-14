using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using Marinex.Services;
using Marinex.Models;

namespace Marinex.Views
{
    public partial class DashboardView : Window
    {
        private AISStreamService _aisService;
        private ObservableCollection<ShipPosition> _shipPositions;
        private Dictionary<string, GMapMarker> _shipMarkers;
        private bool _isStreaming = false;
        private int _messageCount = 0;

        public DashboardView()
        {
            InitializeComponent();
            InitializeMap();
            InitializeData();
            InitializeAISService();
        }

        private void InitializeMap()
        {
            // Configure map
            MainMap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            
            // Set initial position (center of world)
            MainMap.Position = new GMap.NET.PointLatLng(0, 0);
            MainMap.Zoom = 3;
            MainMap.MinZoom = 2;
            MainMap.MaxZoom = 18;
            
            // Enable controls
            MainMap.MouseWheelZoomEnabled = true;
            MainMap.ShowCenter = false;
            MainMap.DragButton = System.Windows.Input.MouseButton.Left;
        }

        private void InitializeData()
        {
            _shipPositions = new ObservableCollection<ShipPosition>();
            _shipMarkers = new Dictionary<string, GMapMarker>();
            ShipDataGrid.ItemsSource = _shipPositions;
        }

        private async void InitializeAISService()
        {
            _aisService = new AISStreamService("1b90478aba10bbb41a3ea8d46274850b65644baf");
            _aisService.OnShipPositionReceived += OnShipPositionReceived;
            _aisService.OnConnectionStatusChanged += OnConnectionStatusChanged;
            
            await StartStreamAsync();
        }

        private async System.Threading.Tasks.Task StartStreamAsync()
        {
            try
            {
                await _aisService.StartStreamAsync();
                _isStreaming = true;
                UpdateConnectionStatus(true, "● Connected");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to AIS Stream: {ex.Message}", 
                              "Connection Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                UpdateConnectionStatus(false, "● Disconnected");
            }
        }

        private void OnShipPositionReceived(object sender, ShipPosition position)
        {
            Dispatcher.Invoke(() =>
            {
                _messageCount++;
                TxtMessagesCount.Text = _messageCount.ToString();
                TxtLastUpdate.Text = DateTime.Now.ToString("HH:mm:ss");

                AddOrUpdateShipMarker(position);
                UpdateShipList(position);
            });
        }

        private void OnConnectionStatusChanged(object sender, bool isConnected)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateConnectionStatus(isConnected, 
                    isConnected ? "● Connected" : "● Disconnected");
            });
        }

        private void AddOrUpdateShipMarker(ShipPosition position)
        {
            var point = new PointLatLng(position.Latitude, position.Longitude);
            
            if (_shipMarkers.ContainsKey(position.Mmsi))
            {
                // Update existing marker
                var marker = _shipMarkers[position.Mmsi];
                marker.Position = point;
            }
            else
            {
                // Create new marker
                var marker = new GMapMarker(point)
                {
                    Shape = CreateShipMarkerShape(position),
                    ZIndex = 100
                };
                
                _shipMarkers[position.Mmsi] = marker;
                MainMap.Markers.Add(marker);
                
                // Update total ships count
                TxtTotalShips.Text = _shipMarkers.Count.ToString();
            }
        }

        private UIElement CreateShipMarkerShape(ShipPosition position)
        {
            var canvas = new System.Windows.Controls.Canvas
            {
                Width = 24,
                Height = 24
            };

            // Ship icon (triangle pointing in direction of course)
            var shipIcon = new Polygon
            {
                Points = new PointCollection(new[]
                {
                    new Point(12, 0),   // Top
                    new Point(24, 20),  // Bottom right
                    new Point(12, 16),  // Bottom center
                    new Point(0, 20)    // Bottom left
                }),
                Fill = Brushes.Red,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            // Rotate based on course
            var rotateTransform = new RotateTransform(position.Course, 12, 12);
            shipIcon.RenderTransform = rotateTransform;

            canvas.Children.Add(shipIcon);
            canvas.ToolTip = $"{position.ShipName}\nMMSI: {position.Mmsi}\nSpeed: {position.Speed:F1} knots";

            return canvas;
        }

        private void UpdateShipList(ShipPosition position)
        {
            // Check if ship already exists in list
            var existingShip = _shipPositions.FirstOrDefault(s => s.Mmsi == position.Mmsi);
            
            if (existingShip != null)
            {
                // Update existing
                existingShip.Latitude = position.Latitude;
                existingShip.Longitude = position.Longitude;
                existingShip.Speed = position.Speed;
                existingShip.Course = position.Course;
                existingShip.LastUpdate = position.LastUpdate;
            }
            else
            {
                // Add new to the top
                _shipPositions.Insert(0, position);
                
                // Keep only last 50 entries
                while (_shipPositions.Count > 50)
                {
                    _shipPositions.RemoveAt(_shipPositions.Count - 1);
                }
            }
        }

        private void UpdateConnectionStatus(bool isConnected, string statusText)
        {
            ConnectionStatus.Text = statusText;
            ConnectionStatus.Foreground = new SolidColorBrush(
                isConnected ? Color.FromRgb(76, 175, 80) : Color.FromRgb(244, 67, 54));
            TxtActiveConnections.Text = isConnected ? "1" : "0";
        }

        private async void BtnToggleStream_Click(object sender, RoutedEventArgs e)
        {
            if (_isStreaming)
            {
                await _aisService.StopStreamAsync();
                _isStreaming = false;
                BtnToggleStream.Content = "Start Stream";
                BtnToggleStream.Background = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                UpdateConnectionStatus(false, "● Disconnected");
            }
            else
            {
                await StartStreamAsync();
                BtnToggleStream.Content = "Stop Stream";
                BtnToggleStream.Background = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            }
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (MainMap.Zoom < MainMap.MaxZoom)
                MainMap.Zoom++;
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (MainMap.Zoom > MainMap.MinZoom)
                MainMap.Zoom--;
        }

        private void BtnResetView_Click(object sender, RoutedEventArgs e)
        {
            MainMap.Position = new PointLatLng(0, 0);
            MainMap.Zoom = 3;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", 
                                       "Confirm Logout", 
                                       MessageBoxButton.YesNo, 
                                       MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _aisService?.StopStreamAsync().Wait();
                
                var loginView = new LoginView();
                loginView.Show();
                this.Close();
            }
        }

        protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_aisService != null)
            {
                await _aisService.StopStreamAsync();
            }
            base.OnClosing(e);
        }
    }
}
