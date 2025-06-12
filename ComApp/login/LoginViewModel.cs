using System.Windows.Input;
using comApp.db;
using System.Text.Json;

namespace comApp.login
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public ICommand LoginCommand => new Command(Login);
        private readonly INavigation _navigation;
        private readonly dbConnection _dbConnection;

        public LoginViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _dbConnection = new dbConnection();
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            var response = await _dbConnection.LoginUser(Email, Password);

            if (response == null || !response.IsSuccess)
            {
                string message = response == null
                    ? "Login failed. Please try again."
                    : response.StatusCode == 401
                        ? "Invalid email or password."
                        : $"Login failed: {response.ErrorMessage}";
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
                return;
            }

            try
            {
                var jsonDoc = JsonDocument.Parse(response.Content);
                JsonElement root = jsonDoc.RootElement;

                // Extract and store token
                if (root.TryGetProperty("token", out JsonElement tokenElement))
                {
                    App.SessionToken = tokenElement.GetString();
                }

                // Extract and store user_id
                if (root.TryGetProperty("userId", out JsonElement userIdElement))
                {
                    App.UserId = userIdElement.GetString();
                }

                var token = App.SessionToken;
                await SecureStorage.SetAsync("session_token", token);

                // Proceed after successful login
                await Application.Current.MainPage.DisplayAlert("Success", "Login successful.", "OK");
                Application.Current.MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
            }
        }
    }
}
