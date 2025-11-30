using System;
using System.Collections.ObjectModel;
using System.Windows;
using Marinex.Models;

namespace Marinex.Views
{
    public partial class ShipDetailWindow : Window
    {
        private ShipPosition _shipPosition;

        // Constructor overload to accept active ships list if available (removed for simplification)
        public ShipDetailWindow(ShipPosition shipPosition, object activeShips = null)
        {
            InitializeComponent();
            _shipPosition = shipPosition;
            LoadShipDetails();
        }

        private void LoadShipDetails()
        {
            if (_shipPosition == null)
            {
                MessageBox.Show("Ship data is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            // Header
            TxtShipNameHeader.Text = _shipPosition.ShipName ?? "Unknown";
            TxtShipType.Text = $"Type: {_shipPosition.ShipType ?? "Unknown"}";

            // Identification
            TxtMmsi.Text = _shipPosition.Mmsi ?? "N/A";
            TxtShipTypeDetail.Text = _shipPosition.ShipType ?? "Unknown";

            // Position & Navigation
            TxtLatitude.Text = $"{_shipPosition.Latitude:F6}°";
            TxtLongitude.Text = $"{_shipPosition.Longitude:F6}°";
            TxtSpeed.Text = $"{_shipPosition.Speed:F1} kn";
            TxtCourse.Text = $"{_shipPosition.Course:F1}°";
            TxtHeading.Text = $"{_shipPosition.Heading:F1}°";

            // Destination
            TxtDestination.Text = string.IsNullOrWhiteSpace(_shipPosition.Destination) 
                ? "Not specified" 
                : _shipPosition.Destination;

            // Status
            TxtLastUpdate.Text = _shipPosition.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Btn3DView_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                var view3D = new Ship3DView(_shipPosition);
                view3D.Owner = this;
                view3D.Show(); // Non-modal so they can keep using the dashboard
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening 3D View: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReportIncident_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Convert MMSI to int for shipId if possible (or use 0/hash as fallback)
                int.TryParse(_shipPosition.Mmsi, out int shipId);

                // Open CreateReportView
                var createReportView = new CreateReportView(1, shipId); // Hardcoded userId 1 for now
                
                // Ideally we should pre-populate the ship name in the report view if shipId is passed
                // Since we reverted to manual text box, the user will have to type it or we could pass it differently
                // For now, just opening the window is consistent with the new manual entry approach.
                
                createReportView.Owner = this;
                createReportView.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening report window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCopyCoordinates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string coordinates = $"{_shipPosition.Latitude:F6}, {_shipPosition.Longitude:F6}";
                
                // Try-catch specifically for Clipboard issues
                try
                {
                    Clipboard.SetText(coordinates);
                    MessageBox.Show(
                        $"Coordinates copied to clipboard:\n{coordinates}", 
                        "Success", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    // Retry once if clipboard is busy
                    System.Threading.Thread.Sleep(100);
                    Clipboard.SetText(coordinates);
                    MessageBox.Show(
                        $"Coordinates copied to clipboard (Retry):\n{coordinates}", 
                        "Success", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to copy coordinates: {ex.Message}\n\nManual Copy: {_shipPosition.Latitude:F6}, {_shipPosition.Longitude:F6}", 
                    "Clipboard Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
