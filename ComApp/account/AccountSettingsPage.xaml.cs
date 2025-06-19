using comApp.login;
using comApp.db;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;

namespace comApp.account;
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
        string userId = App.UserId;
        var response = await _dbConnection.GetUserById(userId);

        if (response == null || !response.IsSuccess)
        {
            await DisplayAlert("Error", "Failed to load user data", "OK");
            return;
        }

        dynamic userData = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);


        firstNameLabel.Text = userData.firstName ?? "No first name";
        lastNameLabel.Text = userData.lastName ?? "No last name";
        descriptionLabel.Text = userData.description ?? "(No description)";
    }


    private async void CheckUser()
    {
        string userId = App.UserId;

        if (userId is null or "")
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