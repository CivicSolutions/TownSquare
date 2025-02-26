using System.Windows.Input;
using comApp.db;

namespace comApp.login
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand => new Command(Login);
        private readonly INavigation _navigation;
        private readonly dbConnection _dbConnection;

        public LoginViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _dbConnection = new dbConnection(); // Corrected constructor call
        }

        private async void Login()
        {
            // Simple validation
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                // Show error message to user
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            // Call the database method to login the user
            var response = await _dbConnection.Login(Email, Password);

            // Handle the response
            if (response.Contains("Error"))
            {
                // Show error message
                await Application.Current.MainPage.DisplayAlert("Error", response, "OK");
            }
            else
            {
                // Successful login, navigate to home or dashboard page
                await Application.Current.MainPage.DisplayAlert("Success", "Login successful.", "OK");
                await _navigation.PushAsync(new MainPage());
            }
        }
    }
}
