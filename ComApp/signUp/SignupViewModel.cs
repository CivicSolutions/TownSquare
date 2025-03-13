using System.Windows.Input;
using comApp.db;
using comApp.login;
using Microsoft.Maui.Controls;

namespace comApp.signUp
{
    public class SignupViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public ICommand SignupCommand => new Command(async () => await Signup());

        private readonly INavigation _navigation;
        private readonly dbConnection _dbConnection;
        private readonly Page _page; 

        public SignupViewModel(INavigation navigation, Page page)
        {
            _navigation = navigation;
            _dbConnection = new dbConnection();
            _page = page; 
        }

        public async Task Signup()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Bio))
            {
                await _page.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await _page.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            var response = await _dbConnection.RegisterUser(Name, Email, Password, Bio);

            if (response.Contains("Error"))
            {
                await _page.DisplayAlert("Error", response, "OK");
            }
            else
            {
                await _page.DisplayAlert("Success", "You have registered successfully.", "OK");
                await _navigation.PushAsync(new Login());
            }
        }
    }
}