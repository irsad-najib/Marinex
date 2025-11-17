using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private ObservableCollection<ShipPosition> _filteredShipPositions;
        private Dictionary<string, GMapMarker> _shipMarkers;
        private bool _isStreaming = false;
        private int _messageCount = 0;
        private string _selectedShipMmsi = null;
    // Map collapse state
    private System.Windows.GridLength _prevMapRowHeight;
    private bool _isMapCollapsed = false;

        public DashboardView()
        {
            InitializeComponent();
            InitializeMap();
            InitializeData();
            InitializeAISService();
            // remember initial map row height for restore
            _prevMapRowHeight = MapRow.Height;
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
            MainMap.ShowCenter = false;
            MainMap.DragButton = System.Windows.Input.MouseButton.Left;
        }

        private void InitializeData()
        {
            _shipPositions = new ObservableCollection<ShipPosition>();
            _filteredShipPositions = new ObservableCollection<ShipPosition>();
            _shipMarkers = new Dictionary<string, GMapMarker>();
            ShipListBox.ItemsSource = _filteredShipPositions;
        }

        private async void InitializeAISService()
        {
            _aisService = new AISStreamService("4c2ceb9bcd370f6cfbcad23f65d106c33a30f6b9");
            _aisService.OnShipPositionReceived += OnShipPositionReceived;
            _aisService.OnConnectionStatusChanged += OnConnectionStatusChanged;
            _aisService.OnError += OnStreamError;
            
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

        private void OnStreamError(object sender, string errorMessage)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateConnectionStatus(false, "● Error");
                TxtLastUpdate.Text = errorMessage;
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
                
                // Update marker color if this ship is selected
                if (position.Mmsi == _selectedShipMmsi)
                {
                    marker.Shape = CreateShipMarkerShape(position, true);
                }
            }
            else
            {
                // Create new marker
                var marker = new GMapMarker(point)
                {
                    Shape = CreateShipMarkerShape(position, false),
                    ZIndex = 100
                };
                
                _shipMarkers[position.Mmsi] = marker;
                MainMap.Markers.Add(marker);
                
                // Update total ships count
                TxtTotalShips.Text = _shipMarkers.Count.ToString();
            }
        }

        private UIElement CreateShipMarkerShape(ShipPosition position, bool isHighlighted = false)
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
                Fill = isHighlighted ? Brushes.Yellow : Brushes.Red,
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
                
                // Keep only last 100 entries
                while (_shipPositions.Count > 100)
                {
                    _shipPositions.RemoveAt(_shipPositions.Count - 1);
                }
            }
            
            // Update filtered list
            ApplyFilter();
        }
        
        private void ApplyFilter()
        {
            var filterText = TxtFilter?.Text?.ToLower() ?? "";
            
            _filteredShipPositions.Clear();
            
            var filtered = string.IsNullOrWhiteSpace(filterText) 
                ? _shipPositions 
                : _shipPositions.Where(s => s.ShipName.ToLower().Contains(filterText));
            
            foreach (var ship in filtered)
            {
                _filteredShipPositions.Add(ship);
            }
        }
        
        private void TxtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Update placeholder visibility
            TxtFilterPlaceholder.Visibility = string.IsNullOrEmpty(TxtFilter.Text) 
                ? Visibility.Visible 
                : Visibility.Collapsed;
            
            ApplyFilter();
        }
        
        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            TxtFilter.Text = "";
        }
        
        private void ShipListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ShipListBox.SelectedItem is ShipPosition selectedShip)
            {
                // Reset previous highlighted ship
                if (_selectedShipMmsi != null && _shipMarkers.ContainsKey(_selectedShipMmsi))
                {
                    var prevPosition = _shipPositions.FirstOrDefault(s => s.Mmsi == _selectedShipMmsi);
                    if (prevPosition != null)
                    {
                        _shipMarkers[_selectedShipMmsi].Shape = CreateShipMarkerShape(prevPosition, false);
                    }
                }
                
                // Highlight new selected ship
                _selectedShipMmsi = selectedShip.Mmsi;
                if (_shipMarkers.ContainsKey(_selectedShipMmsi))
                {
                    var marker = _shipMarkers[_selectedShipMmsi];
                    marker.Shape = CreateShipMarkerShape(selectedShip, true);
                    marker.ZIndex = 200; // Bring to front
                    
                    // Center map on selected ship
                    MainMap.Position = new PointLatLng(selectedShip.Latitude, selectedShip.Longitude);
                    if (MainMap.Zoom < 8)
                    {
                        MainMap.Zoom = 8;
                    }
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

        private void btnCollapseMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isMapCollapsed)
                {
                    // Collapse map row and hide map border
                    _prevMapRowHeight = MapRow.Height;
                    MapRow.Height = new System.Windows.GridLength(0);
                    MapBorder.Visibility = System.Windows.Visibility.Collapsed;
                    btnRestoreMap.Visibility = System.Windows.Visibility.Visible;
                    _isMapCollapsed = true;
                    
                    // Update button appearance
                    btnCollapseMap.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error collapsing map: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRestoreMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isMapCollapsed)
                {
                    // Restore map
                    MapRow.Height = _prevMapRowHeight;
                    MapBorder.Visibility = System.Windows.Visibility.Visible;
                    btnRestoreMap.Visibility = System.Windows.Visibility.Collapsed;
                    btnCollapseMap.Visibility = System.Windows.Visibility.Visible;
                    _isMapCollapsed = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring map: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                
                // Show main window and close dashboard
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is MainWindow mainWindow)
                    {
                        mainWindow.Show();
                        break;
                    }
                }
                
                this.Close();
            }
        }

        private void btnCreateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get current user ID and ship ID (you might need to pass these from login)
                int userId = 1; // TODO: Get from logged in user session
                int? shipId = null; // TODO: Get from user's ship selection

                var createReportView = new CreateReportView(userId, shipId);
                createReportView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Create Report: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnViewReports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get current user ID
                int userId = 1; // TODO: Get from logged in user session

                var reportsView = new ReportsView(userId);
                reportsView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Reports View: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnCheckWeather_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var weatherView = new WeatherView();
                weatherView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Weather View: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnShipTracking_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "AIS Ship Tracking View\n\n" +
                "This feature will show:\n" +
                "- Real-time ship positions on map\n" +
                "- Ship details (MMSI, name, course, speed)\n" +
                "- Track your own ships\n" +
                "- View nearby vessels\n\n" +
                "Coming soon!",
                "Ship Tracking",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnShipDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.Tag is ShipPosition shipPosition)
                {
                    ShowShipDetails(shipPosition);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening ship details: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ShipListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (ShipListBox.SelectedItem is ShipPosition selectedShip)
                {
                    ShowShipDetails(selectedShip);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening ship details: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ShowShipDetails(ShipPosition shipPosition)
        {
            try
            {
                var detailWindow = new ShipDetailWindow(shipPosition);
                detailWindow.Owner = this;
                detailWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error displaying ship details: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
