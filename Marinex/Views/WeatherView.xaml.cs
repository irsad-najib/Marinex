using System;
using System.Windows;

namespace Marinex.Views
{
    public partial class WeatherView : Window
    {
        public WeatherView()
        {
            InitializeComponent();
        }

        private void btnCheckWeather_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtLatitude.Text, out double lat) || 
                !double.TryParse(txtLongitude.Text, out double lon))
            {
                MessageBox.Show(
                    "Please enter valid coordinates.",
                    "Invalid Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validate coordinates range
            if (lat < -90 || lat > 90 || lon < -180 || lon > 180)
            {
                MessageBox.Show(
                    "Coordinates out of range.\nLatitude: -90 to 90\nLongitude: -180 to 180",
                    "Invalid Coordinates",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // TODO: Call weather API (OpenWeatherMap, WeatherAPI, etc.)
            // For now, show mock data
            ShowWeatherData(lat, lon);
        }

        private void ShowWeatherData(double lat, double lon)
        {
            // Hide empty state, show weather info
            pnlEmptyState.Visibility = Visibility.Collapsed;
            pnlWeatherInfo.Visibility = Visibility.Visible;

            // Mock data - TODO: Replace with real API call
            txtCoordinates.Text = $"{lat:F4}°N, {lon:F4}°E";
            txtLocation.Text = GetLocationName(lat, lon);
            
            // Random weather for demo
            var random = new Random();
            var temp = random.Next(15, 35);
            var windSpeed = random.Next(5, 25);
            var waveHeight = random.NextDouble() * 3;

            txtTemperature.Text = $"{temp}°C";
            txtCondition.Text = GetWeatherCondition(temp);
            txtWeatherIcon.Text = GetWeatherIcon(temp);
            txtWind.Text = $"{windSpeed} knots {GetWindDirection()}";
            txtVisibility.Text = $"{random.Next(5, 15)} km";
            txtWaveHeight.Text = $"{waveHeight:F1} meters";
            txtSeaState.Text = GetSeaState(waveHeight);
            txtPressure.Text = $"{random.Next(990, 1030)} hPa";
        }

        private string GetLocationName(double lat, double lon)
        {
            // Simple region detection based on coordinates
            if (lat > 0 && lon > 0)
                return "North Pacific Ocean";
            else if (lat > 0 && lon < 0)
                return "North Atlantic Ocean";
            else if (lat < 0 && lon > 0)
                return "Indian Ocean";
            else
                return "South Atlantic Ocean";
        }

        private string GetWeatherCondition(int temp)
        {
            if (temp < 20) return "Clear Sky";
            else if (temp < 28) return "Partly Cloudy";
            else return "Hot & Sunny";
        }

        private string GetWeatherIcon(int temp)
        {
            if (temp < 20) return "☀️";
            else if (temp < 28) return "⛅";
            else return "🌤️";
        }

        private string GetWindDirection()
        {
            string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            return directions[new Random().Next(directions.Length)];
        }

        private string GetSeaState(double waveHeight)
        {
            if (waveHeight < 0.5) return "Calm";
            else if (waveHeight < 1.25) return "Smooth";
            else if (waveHeight < 2.5) return "Moderate";
            else if (waveHeight < 4) return "Rough";
            else return "Very Rough";
        }
    }
}
