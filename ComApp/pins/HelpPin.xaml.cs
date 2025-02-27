namespace comApp.posts;
using comApp.db;

using MySqlConnector;
using Xamarin.Essentials;

public partial class HelpPin : ContentPage
{
    private dbConnection _dbConnection;
    public HelpPin()
    {
        InitializeComponent();

        _dbConnection = new dbConnection();

        ToolbarItems.Add(new ToolbarItem
        {
            IconImageSource = "close_icon.png",
            Priority = 0,
            Order = ToolbarItemOrder.Primary,
            Command = new Command(OnCloseClicked)
        });
    }

    private async void OnCloseClicked()
    {
        bool result = await DisplayAlert("Close", "Are you sure you want to close?", "Yes", "No");

        if (result)
        {
            await Navigation.PopAsync();
        }
    }

    private async Task<Location> GetLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);
            return location;
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            await DisplayAlert("Error", "Location not supported on device: " + fnsEx.Message, "OK");
            return null;
        }
        catch (PermissionException pEx)
        {
            await DisplayAlert("Error", "Location permission denied: " + pEx.Message, "OK");
            return null;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error getting location: " + ex.Message, "OK");
            return null;
        }
    }

    private async void OnUploadPictureClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Select an image",
            FileTypes = FilePickerFileType.Images,
        });

        if (result != null)
        {
            var stream = await result.OpenReadAsync();
            selectedImage.Source = ImageSource.FromStream(() => stream);
        }
    }

    private void OnTitleEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        string title = e.NewTextValue;
        if (string.IsNullOrWhiteSpace(title))
        {
            titleErrorLabel.Text = "Title cannot be empty";
        }
        else if (title.Length > 50)
        {
            titleErrorLabel.Text = "Title must be maximum 50 characters long";
        }
        else
        {
            titleErrorLabel.Text = string.Empty;
        }
    }

    private void OnDescriptionEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        string description = e.NewTextValue;
        if (string.IsNullOrWhiteSpace(description))
        {
            descriptionErrorLabel.Text = "Description cannot be empty";
        }
        else if (description.Length > 300)
        {
            descriptionErrorLabel.Text = "Description must be maximum 300 characters long";
        }
        else
        {
            descriptionErrorLabel.Text = string.Empty;
        }
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        string title = titleEntry.Text;
        string description = descriptionEditor.Text;

        if (string.IsNullOrWhiteSpace(title) || title.Length > 50)
        {
            titleErrorLabel.Text = string.IsNullOrWhiteSpace(title) ? "Title cannot be empty" : "Title must be maximum 50 characters long";
            return;
        }

        if (string.IsNullOrWhiteSpace(description) || description.Length > 300)
        {
            descriptionErrorLabel.Text = string.IsNullOrWhiteSpace(description) ? "Description cannot be empty" : "Description must be maximum 300 characters long";
            return;
        }

        Location location = await GetLocationAsync();
        if (location == null)
        {
            await DisplayAlert("Error", "Unable to retrieve location.", "OK");
            return;
        }

        try
        {
            await _dbConnection.InsertPin(1, title, description, DateTime.Now, location.Latitude, location.Longitude, 1, 1);
            string message = "Pin added successfully \u2714";
            await DisplayAlert("Success", message, "OK");
            await Navigation.PopAsync();
        }
        catch (MySqlException ex)
        {
            await DisplayAlert("Error", "MySQL error: " + ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        NavigationPage.SetHasNavigationBar(this, false);
    }
}
