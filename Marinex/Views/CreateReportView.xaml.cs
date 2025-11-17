using System;
using System.Windows;
using System.Windows.Controls;
using Marinex.Models;
using Marinex.Services;

namespace Marinex.Views
{
    public partial class CreateReportView : Window
    {
        private readonly ReportService _reportService;
        private readonly int _userId;
        private int? _shipId;

        public CreateReportView(int userId, int? shipId = null)
        {
            InitializeComponent();
            _reportService = new ReportService();
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
                // Detect which tab is active
                var tabControl = (TabControl)this.FindName("TabControl");
                
                // Get selected tab by checking which fields are visible/active
                // For now, let's check which tab has focus or content
                
                // Simple check: try to read pollution fields first
                if (!string.IsNullOrWhiteSpace(txtPollutionLocation.Text))
                {
                    SubmitPollutionReport();
                }
                else if (!string.IsNullOrWhiteSpace(txtSafetyLocation.Text))
                {
                    SubmitSafetyReport();
                }
                else if (!string.IsNullOrWhiteSpace(txtMaintenanceLocation.Text))
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

            // TODO: Save to database via ReportService
            // _reportService.CreatePollutionReport(report);

            MessageBox.Show(
                "Pollution report submitted successfully!",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Show report preview
            MessageBox.Show(
                report.GenerateReport(),
                "Report Preview",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            this.Close();
        }

        private void SubmitSafetyReport()
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

            var report = new SafetyReport
            {
                Location = txtSafetyLocation.Text,
                IncidentType = (cmbIncidentType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Other",
                Severity = (cmbSafetySeverity.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Minor",
                Description = txtIncidentDetails.Text + 
                             (casualties > 0 ? $"\nCasualties: {casualties}" : "") +
                             (!string.IsNullOrWhiteSpace(txtSafetyAction.Text) ? $"\nAction Taken: {txtSafetyAction.Text}" : ""),
                UserID = _userId,
                CreatedAt = DateTime.Now
            };

            MessageBox.Show(
                "Safety report submitted successfully!",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            MessageBox.Show(
                report.GenerateReport(),
                "Report Preview",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            this.Close();
        }

        private void SubmitMaintenanceReport()
        {
            if (string.IsNullOrWhiteSpace(txtMaintenanceLocation.Text))
            {
                MessageBox.Show("Location is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var report = new MaintenanceReport
            {
                Location = txtMaintenanceLocation.Text,
                EquipmentName = txtEquipmentName.Text,
                Priority = (cmbPriority.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Medium",
                IssueDescription = $"Task Type: {(cmbTaskType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Routine Inspection"}\n" +
                                  $"Status: {(cmbMaintenanceStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Scheduled"}\n" +
                                  $"{txtMaintenanceDescription.Text}" +
                                  (!string.IsNullOrWhiteSpace(txtFindings.Text) ? $"\nFindings: {txtFindings.Text}" : ""),
                UserID = _userId,
                ShipID = _shipId ?? 0,
                CreatedAt = DateTime.Now
            };

            MessageBox.Show(
                "Maintenance report submitted successfully!",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            MessageBox.Show(
                report.GenerateReport(),
                "Report Preview",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            this.Close();
        }
    }
}
