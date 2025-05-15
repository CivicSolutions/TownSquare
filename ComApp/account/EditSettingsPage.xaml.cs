namespace comApp.account;
using Microsoft.Maui.Controls;
using comApp.db;
using Newtonsoft.Json;

public partial class EditSettingsPage : ContentPage
{
    public dbConnection _dbConnection;

    public EditSettingsPage()
    {
        InitializeComponent();

        // Subscribe to the TextChanged events for username and bio fields
        usernameEntry.TextChanged += OnUsernameEntryTextChanged;
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

    private void OnUsernameEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        string newUsername = e.NewTextValue;

        if (newUsername.Length > 40)
        {
            usernameErrorLabel.Text = "Username must be maximum 40 characters long";
        }
        else
        {
            usernameErrorLabel.Text = string.Empty;
        }
    }

    private void OnBioEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        string newBio = e.NewTextValue;

        if (newBio.Length > 200)
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
        int userId = App.UserId;

        string response = await _dbConnection.GetUserById(userId);
        dynamic userData = JsonConvert.DeserializeObject(response);

        BindingContext = userData;
    }

    private async void OnSaveChangesClicked(object sender, EventArgs e)
    {
        string newUsername = usernameEntry.Text;
        string newBio = bioEditor.Text;
        int userId = App.UserId;

        // Get current user data to keep email and password unchanged
        string currentUserResponse = await _dbConnection.GetUserById(userId);
        dynamic userData = JsonConvert.DeserializeObject(currentUserResponse);
        string email = userData.email;
        string password = userData.password;

        // Update user with all required fields
        string response = await _dbConnection.UpdateUser(userId, (string)email, (string)password, newUsername, newBio);

        if (!string.IsNullOrEmpty(response))
        {
            await DisplayAlert("Success", "User data updated successfully", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Failed to update user data", "OK");
        }
    }
}
