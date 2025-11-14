using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Marinex.Views;

namespace Marinex;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private bool isLoggedIn = false;

    public MainWindow()
    {
        InitializeComponent();
        
        // Start with Landing view
        NavigateTo(new LandingView());
    }

    private void NavigateTo(UserControl view)
    {
        ContentArea.Content = view;
    }

    private void Logo_Click(object sender, MouseButtonEventArgs e)
    {
        NavigateTo(new LandingView());
    }

    private void Landing_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo(new LandingView());
    }

    private void Ships_Click(object sender, RoutedEventArgs e)
    {
        if (!isLoggedIn)
        {
            MessageBox.Show("Please sign in to access this feature.", "Authentication Required", MessageBoxButton.OK, MessageBoxImage.Information);
            Auth_Click(sender, e);
            return;
        }
        // TODO: Create ShipsView
        MessageBox.Show("Ships view coming soon!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Voyages_Click(object sender, RoutedEventArgs e)
    {
        if (!isLoggedIn)
        {
            MessageBox.Show("Please sign in to access this feature.", "Authentication Required", MessageBoxButton.OK, MessageBoxImage.Information);
            Auth_Click(sender, e);
            return;
        }
        // TODO: Create VoyagesView
        MessageBox.Show("Voyages view coming soon!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Maintenance_Click(object sender, RoutedEventArgs e)
    {
        if (!isLoggedIn)
        {
            MessageBox.Show("Please sign in to access this feature.", "Authentication Required", MessageBoxButton.OK, MessageBoxImage.Information);
            Auth_Click(sender, e);
            return;
        }
        // TODO: Create MaintenanceView
        MessageBox.Show("Maintenance view coming soon!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Auth_Click(object sender, RoutedEventArgs e)
    {
        if (isLoggedIn)
        {
            // Logout
            isLoggedIn = false;
            btnAuth.Content = "Sign In";
            NavigateTo(new LandingView());
            MessageBox.Show("You have been signed out.", "Signed Out", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            // Navigate to login
            var loginView = new LoginView();
            loginView.LoginSuccess += (s, args) =>
            {
                isLoggedIn = true;
                btnAuth.Content = "Sign Out";
                NavigateTo(new LandingView());
                MessageBox.Show("Welcome to Marinex!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            loginView.NavigateToRegister += (s, args) =>
            {
                ShowRegisterView();
            };
            NavigateTo(loginView);
        }
    }

    private void ShowRegisterView()
    {
        var registerView = new RegisterView();
        registerView.RegisterSuccess += (s, args) =>
        {
            // Go back to login after successful registration
            Auth_Click(this, new RoutedEventArgs());
        };
        registerView.NavigateToLogin += (s, args) =>
        {
            Auth_Click(this, new RoutedEventArgs());
        };
        NavigateTo(registerView);
    }
}