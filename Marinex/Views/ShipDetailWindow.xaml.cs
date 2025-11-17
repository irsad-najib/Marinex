using System;
using System.Windows;
using Marinex.Models;

namespace Marinex.Views
{
    public partial class ShipDetailWindow : Window
    {
        private ShipPosition _shipPosition;

        public ShipDetailWindow(ShipPosition shipPosition)
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

        private void BtnCopyCoordinates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string coordinates = $"{_shipPosition.Latitude:F6}, {_shipPosition.Longitude:F6}";
                Clipboard.SetText(coordinates);
                
                MessageBox.Show(
                    $"Coordinates copied to clipboard:\n{coordinates}", 
                    "Success", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to copy coordinates: {ex.Message}", 
                    "Error", 
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
