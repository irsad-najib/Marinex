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

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter your email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                bool isRegistered = await _supabaseService.RegisterUser(
                    txtEmail.Text.Trim(), 
                    txtPassword.Password, 
                    txtUsername.Text.Trim(), 
                    txtCompany.Text.Trim()
                );

                this.IsEnabled = true;
                this.Cursor = Cursors.Arrow;

                if (isRegistered)
                {
                    MessageBox.Show("Registration successful! You can now sign in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    RegisterSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Registration failed. Email might already be in use.", "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                this.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
                MessageBox.Show($"Error connecting to database: {ex.Message}\n\nPlease check your connection string in SupabaseService.cs", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignInLink_Click(object sender, MouseButtonEventArgs e)
        {
            NavigateToLogin?.Invoke(this, EventArgs.Empty);
        }
    }
}
