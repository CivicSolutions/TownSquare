namespace comApp.login;
using comApp.signUp;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
        BindingContext = new LoginViewModel(Navigation);
        NavigationPage.SetHasBackButton(this, false);
    }
    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new signUpPage());
    }
    private async void OnSkipLoginClicked(object sender, EventArgs e)
    {
        // Simulate setting a logged-in user with ID 1
        App.UserId = "eef0d089-669c-4efc-bc4a-51523c5052fa";


        // Optionally preload dummy user data here if needed

        // Navigate to main application page
        await Navigation.PushAsync(new MainPage()); // Replace with your actual main/home page
    }

}