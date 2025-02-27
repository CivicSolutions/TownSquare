using System.Windows.Input;
using comApp.db;
using comApp.login;

namespace comApp.signUp
{
    public class SignupViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand SignupCommand => new Command(async () => await Signup());  // Make sure the command executes the async method
        private readonly INavigation _navigation;
        private readonly dbConnection _dbConnection;

        public SignupViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _dbConnection = new dbConnection();
        }

        public async Task Signup()  // Change return type to Task
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Bio))
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

            // Call the database method to register the user
            var response = await _dbConnection.RegisterUser(Name, Email, Password, Bio);

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
