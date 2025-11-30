using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Marinex.Models;
using Marinex.Services;

namespace Marinex.Views
{
    public partial class CreateReportView : Window
    {
        private readonly ReportService _reportService;
        private readonly MaintenanceAIService _aiService;
        private readonly int _userId;
        private int? _shipId;

        public CreateReportView(int userId, int? shipId = null)
        {
            InitializeComponent();
            _reportService = new ReportService();
            _aiService = new MaintenanceAIService();
            _userId = userId;
            _shipId = shipId;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to cancel? Any unsaved changes will be lost.",
                "Confirm Cancel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnSaveDraft_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Draft functionality coming soon!",
                "Info",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check visible/active tab (simple logic)
                if (!string.IsNullOrWhiteSpace(txtPollutionLocation.Text))
                {
                    SubmitPollutionReport();
                }
                else if (!string.IsNullOrWhiteSpace(txtSafetyLocation.Text))
                {
                    SubmitSafetyReport();
                }
                else if (!string.IsNullOrWhiteSpace(txtEquipmentName.Text)) // Check Maintenance
                {
                    SubmitMaintenanceReport();
                }
                else
                {
                    MessageBox.Show(
                        "Please fill in the required fields.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error submitting report: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void SubmitPollutionReport()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtPollutionLocation.Text))
            {
                MessageBox.Show("Location is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(txtPollutionLatitude.Text, out double lat) || 
                !double.TryParse(txtPollutionLongitude.Text, out double lon))
            {
                MessageBox.Show("Please enter valid coordinates.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var report = new PollutionReport
            {
                Location = txtPollutionLocation.Text,
                Latitude = lat,
                Longitude = lon,
                WasteType = (cmbWasteType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Unknown",
                Quantity = (cmbQuantity.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Unknown",
                Severity = (cmbSeverity.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Low",
                Description = txtPollutionDescription.Text,
                EnvironmentalImpact = txtEnvironmentalImpact.Text,
                Status = "Reported",
                UserID = _userId,
                ShipID = _shipId,
                CreatedAt = DateTime.Now
            };

            // Add to PollutionDataService (Local + DB)
            SubmitReportAsync(report);
        }

        private string RunAIAnalysis(MaintenanceReport report)
        {
            // Simulate ship context based on inputs
            var ship = new Ship { ShipID = report.ShipID, ShipName = "Target Ship" };
            var history = new List<Maintenance>();
            
            // Default values
            double engineHours = 2500;
            double fuelConsumption = 100;

            // Adjust based on user inputs to give meaningful feedback
            string priority = report.Priority.ToLower();
            if (priority == "urgent" || priority == "high")
            {
                engineHours = 5500;
                fuelConsumption = 125;
                history.Add(new Maintenance { Date = DateTime.Now.AddDays(-150) });
            }
            else
            {
                history.Add(new Maintenance { Date = DateTime.Now.AddDays(-45) });
            }

            var prediction = _aiService.PredictMaintenance(ship, history, engineHours, fuelConsumption);
            
            return $"Risk Level: {prediction.RiskLevel}\n" +
                   $"Recommendation: {prediction.RecommendedAction}\n" +
                   $"Estimated Cost: ${prediction.EstimatedCost:N0}";
        }

        private async void SubmitReportAsync(PollutionReport report)
        {
            try
            {
                await PollutionDataService.Instance.AddReportAsync(report);

                MessageBox.Show(
                    "Pollution report submitted successfully! Saved to Database & Map updated.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                    
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving report: {ex.Message}");
            }
        }

        private async void SubmitSafetyReport()
        {
            if (string.IsNullOrWhiteSpace(txtSafetyLocation.Text))
            {
                MessageBox.Show("Location is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtCasualties.Text, out int casualties))
            {
                casualties = 0;
            }

            // Get ship name from manual text box
            string selectedShipName = cmbSafetyTargetShip.Text;
            if (string.IsNullOrWhiteSpace(selectedShipName))
            {
                selectedShipName = "Unknown Ship";
            }

            var report = new SafetyReport
            {
                Location = txtSafetyLocation.Text,
                IncidentType = (cmbIncidentType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Other",
                Severity = (cmbSafetySeverity.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Minor",
                Description = txtIncidentDetails.Text + 
                             (casualties > 0 ? $"\nCasualties: {casualties}" : "") +
                             (!string.IsNullOrWhiteSpace(txtSafetyAction.Text) ? $"\nAction Taken: {txtSafetyAction.Text}" : "") +
                             $"\n(Ship Involved: {selectedShipName})", 
                UserID = _userId,
                CreatedAt = DateTime.Now
            };

            try 
            {
                await PollutionDataService.Instance.SaveSafetyReportAsync(report);
                
                MessageBox.Show(
                    "Safety report submitted successfully! Saved to Database.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving safety report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SubmitMaintenanceReport()
        {
            if (string.IsNullOrWhiteSpace(txtMaintenanceLocation.Text))
            {
                MessageBox.Show("Location is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get ship name from manual text box
            string selectedShipName = cmbTargetShip.Text;
            int selectedShipId = _shipId ?? 0; // Use passed shipId or 0 if not provided/manual
            
            if (string.IsNullOrWhiteSpace(selectedShipName))
            {
                selectedShipName = "Unknown Ship";
            }

            var report = new MaintenanceReport
            {
                Location = txtMaintenanceLocation.Text,
                EquipmentName = txtEquipmentName.Text + $" (Ship: {selectedShipName})", 
                Priority = (cmbPriority.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Medium",
                IssueDescription = $"Task Type: {(cmbTaskType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Routine Inspection"}\n" +
                                  $"Status: {(cmbMaintenanceStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Scheduled"}\n" +
                                  $"{txtMaintenanceDescription.Text}" +
                                  (!string.IsNullOrWhiteSpace(txtFindings.Text) ? $"\nFindings: {txtFindings.Text}" : ""),
                UserID = _userId,
                ShipID = selectedShipId,
                CreatedAt = DateTime.Now
            };

            // Run AI Analysis before saving
            try 
            {
                string aiAnalysis = RunAIAnalysis(report);
                report.IssueDescription += $"\n\n[AI ANALYSIS]\n{aiAnalysis}";
            }
            catch 
            {
                // Ignore AI errors during submission to not block user
            }

            try 
            {
                await PollutionDataService.Instance.SaveMaintenanceReportAsync(report);
                
                MessageBox.Show(
                    "Maintenance report submitted successfully! Saved to Database.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving maintenance report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
