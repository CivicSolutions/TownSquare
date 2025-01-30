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
}