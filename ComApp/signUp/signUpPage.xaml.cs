namespace comApp.signUp;
using comApp.login;
public partial class signUpPage : ContentPage
{
	public signUpPage()
	{
		InitializeComponent();
        BindingContext = new SignupViewModel(Navigation);
        NavigationPage.SetHasBackButton(this, false);
    }
    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Login());
    }
}