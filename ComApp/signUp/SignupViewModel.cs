using System.Windows.Input;
using comApp.db;
using comApp.login;

namespace comApp.signUp
{
    public class SignupViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand SignupCommand => new Command(Signup);
        private readonly INavigation _navigation;
        private readonly dbConnection _dbConnection;

        public SignupViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _dbConnection = new dbConnection();
        }

        private async void Signup()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Description))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            var response = await _dbConnection.RegisterUser(Name, Email, Password, Description);

            if (!response.IsSuccess)
            {
                string message = response.StatusCode == 401
                    ? "You are not authorized to register."
                    : $"Registration failed: {response.ErrorMessage}";
                await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Success", "You have registered successfully.", "OK");
                await _navigation.PushAsync(new Login());
            }
        }
    }
}
