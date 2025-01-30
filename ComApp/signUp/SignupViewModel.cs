using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string confirmPassword {  get; set; }

        public ICommand SignupCommand => new Command(Signup);
        private readonly INavigation _navigation;

        public SignupViewModel(INavigation navigation)
        {
            _navigation = navigation;
        }
        private async void Signup()
        {
            dbConnection db = new dbConnection();
            db.InsertUser(Name, Email, Bio, Password);
            // Perform signup logic here
            // For simplicity, let's assume signup is successful if all fields are not empty
            if (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password))
            {
                // Perform database insertion here (using the provided insert query)
                // Display a success message upon successful signup
                await Application.Current.MainPage.DisplayAlert("Success", "Sign up successful", "OK");
                await _navigation.PushAsync(new Login());
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields", "OK");
            }
        }
    }
}
