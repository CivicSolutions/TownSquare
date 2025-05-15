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
                // Show error message to user
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                // Show error if passwords don't match
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            //         public Task<string> RegisterUser(int id, string name, string email, string password, string description)
            var response = await _dbConnection.RegisterUser(Name, Email, Password, Description);

            // Handle the response
            if (response.Contains("Error"))
            {
                await Application.Current.MainPage.DisplayAlert("Error", response, "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Success", "You have registered successfully.", "OK");
                await _navigation.PushAsync(new Login());
            }
        }
    }

}
