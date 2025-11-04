using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Marinex.Services;

namespace Marinex.Views
{
    public partial class RegisterView : UserControl
    {
        private readonly SupabaseService _supabaseService;
        public event EventHandler? RegisterSuccess;
        public event EventHandler? NavigateToLogin;

        public RegisterView()
        {
            InitializeComponent();
            _supabaseService = new SupabaseService();
        }

        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter your username.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCompany.Text))
            {
                MessageBox.Show("Please enter your company name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!chkTerms.IsChecked.GetValueOrDefault())
            {
                MessageBox.Show("Please agree to the Terms and Conditions.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                this.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                // Schema database: hanya perlu UserName, Password, Company (tidak ada email)
                var (isRegistered, errorMessage) = await _supabaseService.RegisterUser(
                    txtUsername.Text.Trim(), 
                    txtPassword.Password, 
                    txtCompany.Text.Trim()
                );

                this.IsEnabled = true;
                this.Cursor = Cursors.Arrow;

                if (isRegistered)
                {
                    MessageBox.Show("Registrasi berhasil! Silakan login dengan akun Anda.", "Berhasil", MessageBoxButton.OK, MessageBoxImage.Information);
                    RegisterSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show(errorMessage, "Registrasi Gagal", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                this.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
                MessageBox.Show($"Error: {ex.Message}\n\nSilakan cek koneksi internet dan connection string di SupabaseService.cs", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignInLink_Click(object sender, MouseButtonEventArgs e)
        {
            NavigateToLogin?.Invoke(this, EventArgs.Empty);
        }
    }
}
