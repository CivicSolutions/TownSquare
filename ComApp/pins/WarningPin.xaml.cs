using MySqlConnector;
using comApp.db;

namespace comApp.posts;

public partial class WarningPin : ContentPage
{
    private dbConnection _dbConnection;

    public WarningPin()
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

    private async void OnUploadPictureClicked(object sender, EventArgs e)
    {
        var result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Select an image",
            FileTypes = FilePickerFileType.Images,
        });

        if (result != null)
        {
            selectedImage.Source = ImageSource.FromFile(result.FullPath);
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
            Console.WriteLine("Location not supported on device: " + fnsEx.Message);
            return null;
        }
        catch (PermissionException pEx)
        {
            Console.WriteLine("Location permission denied: " + pEx.Message);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting location: " + ex.Message);
            return null;
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
            var response = await _dbConnection.CreatePin(
                title,
                description,
                (double)location.Latitude,
                (double)location.Longitude,
                1, // hardcoded communityid for now
                2  // pintype for warning
            );

            if (response == null || !response.IsSuccess)
            {
                string message = response == null
                    ? "An error occurred while adding the pin."
                    : response.StatusCode == 401
                        ? "You are not authorized to add a pin."
                        : $"An error occurred: {response.ErrorMessage}";
                await DisplayAlert("Error", message, "OK");
            }
            else
            {
                string message = "Pin added successfully \u2714";
                await DisplayAlert("Success", message, "OK");
                await Navigation.PopAsync();
            }
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
