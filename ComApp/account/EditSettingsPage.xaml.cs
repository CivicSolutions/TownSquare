namespace comApp.account;
using Microsoft.Maui.Controls;
using comApp.db;
using Newtonsoft.Json;

public partial class EditSettingsPage : ContentPage
{
    private dbConnection _dbConnection;

    public EditSettingsPage()
    {
        InitializeComponent();

        bioEditor.TextChanged += OnBioEditorTextChanged;

        ToolbarItems.Add(new ToolbarItem
        {
            IconImageSource = "close_icon.png",
            Priority = 0,
            Order = ToolbarItemOrder.Primary,
            Command = new Command(OnCloseClicked)
        });

        _dbConnection = new dbConnection();
        BindingContext = this;
        LoadUser();
    }

    private async void OnCloseClicked()
    {
        bool result = await DisplayAlert("Close", "Are you sure you want to close?", "Yes", "No");

        if (result)
        {
            await Navigation.PopAsync();
        }
    }

    private void OnBioEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        string description = e.NewTextValue;

        if (description.Length > 200)
        {
            bioErrorLabel.Text = "Bio must be maximum 200 characters long";
        }
        else
        {
            bioErrorLabel.Text = string.Empty;
        }
    }

    private async void LoadUser()
    {
        string userId = App.UserId;

        var response = await _dbConnection.GetUserById(userId);

        if (response == null || !response.IsSuccess)
        {
            string message = response == null
                ? "Failed to load user data."
                : response.StatusCode == 401
                    ? "You are not authorized to view user data."
                    : $"Failed to load user data: {response.ErrorMessage}";
            await DisplayAlert("Error", message, "OK");
            return;
        }

        dynamic userData = JsonConvert.DeserializeObject(response.Content);

        firstNameEntry.Text = userData.firstName;
        lastNameEntry.Text = userData.lastName;
        bioEditor.Text = userData.description;
    }

    private async void OnSaveChangesClicked(object sender, EventArgs e)
    {
        string firstName = firstNameEntry.Text?.Trim();
        string lastName = lastNameEntry.Text?.Trim();
        string description = bioEditor.Text?.Trim();
        string userId = App.UserId;

        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            await DisplayAlert("Error", "First and last name cannot be empty.", "OK");
            return;
        }

        // Get current user data to keep email and password unchanged
        var currentUserResponse = await _dbConnection.GetUserById(userId);

        if (currentUserResponse == null || !currentUserResponse.IsSuccess)
        {
            string message = currentUserResponse == null
                ? "Failed to load user data."
                : currentUserResponse.StatusCode == 401
                    ? "You are not authorized to update user data."
                    : $"Failed to load user data: {currentUserResponse.ErrorMessage}";
            await DisplayAlert("Error", message, "OK");
            return;
        }

        dynamic userData = JsonConvert.DeserializeObject(currentUserResponse.Content);
        string email = userData.email;
        string password = userData.password;

        var response = await _dbConnection.UpdateUser(userId, (string)email, firstName, lastName, description);

        if (response != null && response.IsSuccess)
        {
            await DisplayAlert("Success", "User data updated successfully", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            string message = response == null
                ? "Failed to update user data."
                : response.StatusCode == 401
                    ? "You are not authorized to update user data."
                    : $"Failed to update user data: {response.ErrorMessage}";
            await DisplayAlert("Error", message, "OK");
        }
    }
}
