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
        private PollutionDataService _pollutionService;
        private ObservableCollection<ShipPosition> _shipPositions;
        private ObservableCollection<ShipPosition> _filteredShipPositions;
        private Dictionary<string, GMapMarker> _shipMarkers;
        private List<GMapMarker> _pollutionMarkers;
        private bool _isStreaming = false;
        private bool _isPollutionVisible = false;
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
            InitializeServices();
            _prevMapRowHeight = MapRow.Height;
        }

        private void InitializeMap()
        {
            MainMap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            MainMap.Position = new GMap.NET.PointLatLng(0, 0);
            MainMap.Zoom = 3;
            MainMap.MinZoom = 2;
            MainMap.MaxZoom = 18;
            MainMap.ShowCenter = false;
            MainMap.DragButton = System.Windows.Input.MouseButton.Left;
        }

        private void InitializeData()
        {
            _shipPositions = new ObservableCollection<ShipPosition>();
            _filteredShipPositions = new ObservableCollection<ShipPosition>();
            _shipMarkers = new Dictionary<string, GMapMarker>();
            _pollutionMarkers = new List<GMapMarker>();
            ShipListBox.ItemsSource = _filteredShipPositions;
        }

        private async void InitializeServices()
        {
            _pollutionService = PollutionDataService.Instance;
            _pollutionService.OnReportAdded += OnPollutionReportAdded;

            _aisService = new AISStreamService("4c2ceb9bcd370f6cfbcad23f65d106c33a30f6b9");
            _aisService.OnShipPositionReceived += OnShipPositionReceived;
            _aisService.OnConnectionStatusChanged += OnConnectionStatusChanged;
            _aisService.OnError += OnStreamError;
            
            await StartStreamAsync();
        }

        private void OnPollutionReportAdded(object sender, PollutionReport newReport)
        {
            Dispatcher.Invoke(() =>
            {
                if (_isPollutionVisible)
                {
                    var marker = CreateHeatmapMarker(newReport);
                    _pollutionMarkers.Add(marker);
                    MainMap.Markers.Add(marker);
                    MainMap.Position = new PointLatLng(newReport.Latitude, newReport.Longitude);
                    MainMap.Zoom = 10;
                    MessageBox.Show("New pollution report added to map!", "Live Update", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });
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
                var marker = _shipMarkers[position.Mmsi];
                marker.Position = point;
                if (position.Mmsi == _selectedShipMmsi)
                {
                    marker.Shape = CreateShipMarkerShape(position, true);
                }
            }
            else
            {
                var marker = new GMapMarker(point)
                {
                    Shape = CreateShipMarkerShape(position, false),
                    ZIndex = 100
                };
                _shipMarkers[position.Mmsi] = marker;
                MainMap.Markers.Add(marker);
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

            var shipIcon = new Polygon
            {
                Points = new PointCollection(new[]
                {
                    new Point(12, 0),
                    new Point(24, 20),
                    new Point(12, 16),
                    new Point(0, 20)
                }),
                Fill = isHighlighted ? Brushes.Yellow : Brushes.Red,
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            var rotateTransform = new RotateTransform(position.Course, 12, 12);
            shipIcon.RenderTransform = rotateTransform;

            canvas.Children.Add(shipIcon);
            canvas.ToolTip = $"{position.ShipName}\nMMSI: {position.Mmsi}\nSpeed: {position.Speed:F1} knots";
            
            // Make canvas interactive
            canvas.Background = Brushes.Transparent; // Hit test visible area
            canvas.Cursor = System.Windows.Input.Cursors.Hand;
            canvas.MouseLeftButtonUp += (s, e) =>
            {
                ShowShipDetails(position);
                e.Handled = true;
            };

            return canvas;
        }

        private async void btnTogglePollution_Click(object sender, RoutedEventArgs e)
        {
            if (_isPollutionVisible)
            {
                foreach (var marker in _pollutionMarkers)
                {
                    MainMap.Markers.Remove(marker);
                }
                _pollutionMarkers.Clear();
                btnTogglePollution.Content = "🗑️ Show Pollution Heatmap";
                btnTogglePollution.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E91E63"));
                _isPollutionVisible = false;
            }
            else
            {
                var pollutionData = await _pollutionService.LoadPollutionDataAsync();
                if (pollutionData.Count == 0)
                {
                    MessageBox.Show("No pollution data available to display.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                foreach (var item in pollutionData)
                {
                    var marker = CreateHeatmapMarker(item);
                    _pollutionMarkers.Add(marker);
                    MainMap.Markers.Add(marker);
                }
                
                btnTogglePollution.Content = "🚫 Hide Pollution Heatmap";
                btnTogglePollution.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C2185B"));
                _isPollutionVisible = true;
                MessageBox.Show($"Loaded {pollutionData.Count} marine debris hotspots (Real + Reported).", "Data Loaded", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private GMapMarker CreateHeatmapMarker(PollutionReport report)
        {
            var point = new PointLatLng(report.Latitude, report.Longitude);
            var marker = new GMapMarker(point);

            double size = report.Severity == "Critical" ? 150 : (report.Severity == "High" ? 100 : 60);
            
            var gradient = new RadialGradientBrush();
            gradient.GradientOrigin = new Point(0.5, 0.5);
            gradient.Center = new Point(0.5, 0.5);
            gradient.RadiusX = 0.5;
            gradient.RadiusY = 0.5;

            Color centerColor = (report.Severity == "Critical" || report.Severity == "High") 
                ? Color.FromArgb(180, 255, 0, 0) 
                : Color.FromArgb(180, 255, 165, 0); 
                
            gradient.GradientStops.Add(new GradientStop(centerColor, 0.0));
            gradient.GradientStops.Add(new GradientStop(Color.FromArgb(50, centerColor.R, centerColor.G, centerColor.B), 0.6));
            gradient.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));

            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = gradient,
                IsHitTestVisible = true,
                Cursor = System.Windows.Input.Cursors.Hand,
                ToolTip = new ToolTip
                {
                    Content = new TextBlock
                    {
                        Text = $"[POLLUTION]\nType: {report.WasteType}\nSeverity: {report.Severity}\nClick for details",
                        Foreground = Brushes.Black
                    }
                }
            };

            ellipse.MouseLeftButtonUp += (s, e) => 
            {
                ShowPollutionDetails(report);
                e.Handled = true;
            };

            marker.Offset = new Point(-size / 2, -size / 2);
            marker.Shape = ellipse;
            marker.ZIndex = 50;

            return marker;
        }

        private void ShowPollutionDetails(PollutionReport report)
        {
            string detailMsg = $"🗑️ POLLUTION REPORT DETAILS\n" +
                               $"{new string('-', 40)}\n" +
                               $"📍 Location: {report.Location}\n" +
                               $"🌐 Coordinates: {report.Latitude:F6}, {report.Longitude:F6}\n\n" +
                               $"⚠️ Type: {report.WasteType}\n" +
                               $"🔥 Severity: {report.Severity}\n" +
                               $"📊 Status: {report.Status}\n\n" +
                               $"📝 Description:\n{report.Description}\n\n" +
                               $"📅 Reported On: {report.CreatedAt:yyyy-MM-dd HH:mm:ss}";

            MessageBox.Show(detailMsg, "Pollution Details", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void UpdateShipList(ShipPosition position)
        {
            var existingShip = _shipPositions.FirstOrDefault(s => s.Mmsi == position.Mmsi);
            if (existingShip != null)
            {
                existingShip.Latitude = position.Latitude;
                existingShip.Longitude = position.Longitude;
                existingShip.Speed = position.Speed;
                existingShip.Course = position.Course;
                existingShip.LastUpdate = position.LastUpdate;
                // Important: Notify UI of property changes if ObservableCollection doesn't handle deep updates automatically
            }
            else
            {
                _shipPositions.Insert(0, position);
                while (_shipPositions.Count > 100)
                {
                    _shipPositions.RemoveAt(_shipPositions.Count - 1);
                }
            }
            
            // Re-apply filter only if necessary to avoid UI flickering or excessive updates
            // ApplyFilter(); 
            
            // Instead of full re-filter, just ensure the new/updated ship respects current filter
            var filterText = TxtFilter?.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(filterText))
            {
                if (!_filteredShipPositions.Contains(position) && _shipPositions.Contains(position))
                {
                     // If no filter, ensure it's in the visible list (if it's new)
                     if (!_filteredShipPositions.Any(s => s.Mmsi == position.Mmsi))
                     {
                         _filteredShipPositions.Insert(0, position);
                     }
                }
            }
            else
            {
                // If there is a filter
                if (position.ShipName.ToLower().Contains(filterText))
                {
                     // If matches filter but not in visible list, add it
                     if (!_filteredShipPositions.Any(s => s.Mmsi == position.Mmsi))
                     {
                         _filteredShipPositions.Insert(0, position);
                     }
                }
                else
                {
                    // If doesn't match filter but IS in visible list, remove it
                    var visibleItem = _filteredShipPositions.FirstOrDefault(s => s.Mmsi == position.Mmsi);
                    if (visibleItem != null)
                    {
                        _filteredShipPositions.Remove(visibleItem);
                    }
                }
            }
            
            // Limit visible list size too to prevent UI lag
            while (_filteredShipPositions.Count > 100)
            {
                _filteredShipPositions.RemoveAt(_filteredShipPositions.Count - 1);
            }
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
                if (_selectedShipMmsi != null && _shipMarkers.ContainsKey(_selectedShipMmsi))
                {
                    var prevPosition = _shipPositions.FirstOrDefault(s => s.Mmsi == _selectedShipMmsi);
                    if (prevPosition != null)
                    {
                        _shipMarkers[_selectedShipMmsi].Shape = CreateShipMarkerShape(prevPosition, false);
                    }
                }
                _selectedShipMmsi = selectedShip.Mmsi;
                if (_shipMarkers.ContainsKey(_selectedShipMmsi))
                {
                    var marker = _shipMarkers[_selectedShipMmsi];
                    marker.Shape = CreateShipMarkerShape(selectedShip, true);
                    marker.ZIndex = 200;
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
            if (MainMap.Zoom < MainMap.MaxZoom) MainMap.Zoom++;
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (MainMap.Zoom > MainMap.MinZoom) MainMap.Zoom--;
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
                    _prevMapRowHeight = MapRow.Height;
                    MapRow.Height = new System.Windows.GridLength(0);
                    MapBorder.Visibility = System.Windows.Visibility.Collapsed;
                    btnRestoreMap.Visibility = System.Windows.Visibility.Visible;
                    _isMapCollapsed = true;
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
            var result = MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _aisService?.StopStreamAsync().Wait();
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
                var reportsView = new ReportsView(1);
                reportsView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Reports View: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Error opening Weather View: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegisterShip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Pass current user ID if needed
                var registerShipView = new RegisterShipView(1); 
                registerShipView.Owner = this;
                registerShipView.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Register Ship: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                MessageBox.Show($"Error opening ship details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Error opening ship details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowShipDetails(ShipPosition shipPosition)
        {
            try
            {
                // Pass current active ships list to the details window
                // This allows the "Report Incident" button to open CreateReportView with the ship list populated
                var detailWindow = new ShipDetailWindow(shipPosition, _shipPositions);
                detailWindow.Owner = this;
                detailWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying ship details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_aisService != null)
            {
                await _aisService.StopStreamAsync();
            }
            if (_pollutionService != null)
            {
                _pollutionService.OnReportAdded -= OnPollutionReportAdded;
            }
            base.OnClosing(e);
        }
    }
}
