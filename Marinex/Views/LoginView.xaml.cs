using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Marinex.Services;

namespace Marinex.Views
{
    public partial class LoginView : UserControl
    {
        private readonly SupabaseService _supabaseService;
        public event EventHandler? LoginSuccess;
        public event EventHandler? NavigateToRegister;

        public LoginView()
        {
            InitializeComponent();
            _supabaseService = new SupabaseService();
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter your username.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                bool isAuthenticated = await _supabaseService.AuthenticateUser(txtUsername.Text.Trim(), txtPassword.Password);

                this.IsEnabled = true;
                this.Cursor = Cursors.Arrow;

                if (isAuthenticated)
                {
                    LoginSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Please try again.", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                this.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
                MessageBox.Show($"Error connecting to database: {ex.Message}\n\nPlease check your connection string in SupabaseService.cs", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SignUpLink_Click(object sender, MouseButtonEventArgs e)
        {
            NavigateToRegister?.Invoke(this, EventArgs.Empty);
        }
    }
}
