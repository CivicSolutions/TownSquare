using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using comApp.db;
using comApp;
using comApp.signUp;
using System.Collections.ObjectModel;
using System.Transactions;

namespace comApp.login
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand => new Command(Login);
        private readonly INavigation _navigation;

        public LoginViewModel(INavigation navigation)
        {
            _navigation = navigation;
        }
        private async void Login()
        {
            // Perform authentication logic here
            // For simplicity, let's assume authentication is successful if email and password are not empty
            if (!string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
            {
                // Check if the user exists in the database
                dbConnection db = new dbConnection();
                bool isAuthenticated = db.SelectUserByEmailAndPassword(Email, Password);

                if (isAuthenticated)
                {
                    // Start user session and navigate to the next page upon successful login
                    StartUserSession();

                    // You can navigate to the next page upon successful login
                    Application.Current.MainPage.DisplayAlert("Success", "Login successful", "OK");
                    await Shell.Current.GoToAsync("//MainPage");
                    //await _navigation.PushAsync(new MainPage());
                    // Perform navigation to the next page here
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Error", "Invalid email or password", "OK");
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Error", "Please enter email and password", "OK");
            }
        }

            private void StartUserSession()
        {
            dbConnection db = new dbConnection();
            int userId = db.GetUserIdByEmail(Email); // Assuming you have a method to retrieve user ID by email

            // Save the user's ID to a session using Xamarin.Essentials Preferences
            Preferences.Set("UserId", userId.ToString());
            Application.Current.MainPage.DisplayAlert("User ID", $"The user's ID is: {userId}", "OK"); //--------------------------------------------Delete on Full deployment-----------------------------------------

        }
    }
}