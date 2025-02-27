namespace comApp.account;
using comApp.login;
using comApp.db;
using Microsoft.Maui.Controls;

public partial class AccountSettingsPage : ContentPage
{
    public dbConnection _dbConnection;

    public AccountSettingsPage()
    {
        InitializeComponent();
        _dbConnection = new dbConnection();
        BindingContext = this;
        LoadUser();
    }

    private async void LoadUser()
    {
        int userId = App.UserId;
        var userData = await _dbConnection.GetUserById(userId);

        BindingContext = userData;
    }

    private async void CheckUser()
    {
        int userId = App.UserId;

        if (userId < 0)
        {
            await Navigation.PushAsync(new Login());
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditSettingsPage());
    }

    private async void OnLogoutButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();

        // Navigate back to the Main page
        int userId = -1;
        Preferences.Set("UserId", userId.ToString());
        await Shell.Current.GoToAsync("//LoginPage");
    }

    protected override void OnAppearing()
    {
        LoadUser();
        base.OnAppearing();
        CheckUser();
        NavigationPage.SetHasNavigationBar(this, false);
    }
}
