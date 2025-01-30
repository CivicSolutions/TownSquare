namespace comApp.account;
using Microsoft.Maui.Controls;
using comApp.db;

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

        // Error handling for username length constraint
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

        // Error handling for bio length constraint
        if (newBio.Length > 200)
        {
            bioErrorLabel.Text = "Bio must be maximum 200 characters long";
        }
        else
        {
            bioErrorLabel.Text = string.Empty;
        }
    }

    private void LoadUser()
    {
        int userId = _dbConnection.GetUserIdFromSession();

        var userData = _dbConnection.GetUserById(userId);

        BindingContext = userData;
    }

    private async void OnSaveChangesClicked(object sender, EventArgs e)
    {
        // Get the new username and bio from the UI
        string newUsername = usernameEntry.Text;
        string newBio = bioEditor.Text;

        // Update the user data in the database
        int userId = _dbConnection.GetUserIdFromSession();
        _dbConnection.UpdateUser(userId, newUsername, newBio);

        // Navigate back to the previous page
        await Navigation.PopAsync();
    }

}