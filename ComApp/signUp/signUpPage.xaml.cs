using comApp.signUp;
using Microsoft.Maui.Controls;

namespace comApp.signUp
{
    public partial class signUpPage : ContentPage
    {
        public signUpPage()
        {
            InitializeComponent();
            BindingContext = new SignupViewModel(Navigation, this); // Pass 'this' as the Page
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new login.Login()); // Navigate to login page
        }
    }
}