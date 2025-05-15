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
            _dbConnection = new dbConnection(); // Corrected constructor call
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            var response = await _dbConnection.LoginUser(Email, Password);

            if (response.Contains("Error"))
            {
                await Application.Current.MainPage.DisplayAlert("Error", response, "OK");
                return;
            }

            try
            {
                var jsonDoc = JsonDocument.Parse(response);
                JsonElement root = jsonDoc.RootElement;

                // Extract session ID
                if (root.TryGetProperty("token", out JsonElement tokenElement))
                {
                    App.SessionToken = tokenElement.GetString(); // Save session globally

                    await Application.Current.MainPage.DisplayAlert("Success", "Login successful.", "OK");

                    //// Optionally: Fetch and store userId using session
                    //string userIdResponse = await _dbConnection.GetUserIdFromSession(App.SessionId);
                    //if (int.TryParse(userIdResponse, out int userId))
                    //{
                    //    App.UserId = userId;
                    //}

                    await _navigation.PushAsync(new MainPage());
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Session ID not found in response.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to parse response: {ex.Message}", "OK");
            }
        }

    }
}
