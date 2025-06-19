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
            Console.WriteLine($"Selected image path: {result.FullPath}");
        }
    }

    private async Task<Location> GetLocationAsync()
    {
        try
        {
            Console.WriteLine("Requesting location...");
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);
            Console.WriteLine($"Location received: Lat={location?.Latitude}, Lon={location?.Longitude}");
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
            Console.WriteLine("Error getting location: " + ex.ToString());
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
        Console.WriteLine("Submit clicked.");

        string title = titleEntry.Text;
        string description = descriptionEditor.Text;
        string userId = App.UserId;

        Console.WriteLine($"Input values: title='{title}', description length={description?.Length}, userId='{userId}'");

        if (string.IsNullOrWhiteSpace(title) || title.Length > 50)
        {
            titleErrorLabel.Text = string.IsNullOrWhiteSpace(title) ? "Title cannot be empty" : "Title must be maximum 50 characters long";
            Console.WriteLine("Title validation failed.");
            return;
        }

        if (string.IsNullOrWhiteSpace(description) || description.Length > 300)
        {
            descriptionErrorLabel.Text = string.IsNullOrWhiteSpace(description) ? "Description cannot be empty" : "Description must be maximum 300 characters long";
            Console.WriteLine("Description validation failed.");
            return;
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            Console.WriteLine("UserId is null or empty.");
            await DisplayAlert("Error", "User ID is missing. Please log in again.", "OK");
            return;
        }

        Location location = null;
        try
        {
            location = await GetLocationAsync();
            if (location == null)
            {
                Console.WriteLine("Location is null after GetLocationAsync.");
                await DisplayAlert("Error", "Unable to retrieve location.", "OK");
                return;
            }
        }
        catch (Exception locEx)
        {
            Console.WriteLine("Exception during GetLocationAsync: " + locEx.ToString());
            await DisplayAlert("Error", "Unable to retrieve location due to error.", "OK");
            return;
        }

        try
        {
            Console.WriteLine("Calling CreatePin with parameters:");
            Console.WriteLine($"Title: {title}");
            Console.WriteLine($"Description: {description}");
            Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
            Console.WriteLine($"CommunityId: 1");
            Console.WriteLine($"PinType: 2");
            Console.WriteLine($"UserId: {userId}");

            var response = await _dbConnection.CreatePin(
                title,
                description,
                (double)location.Latitude,
                (double)location.Longitude,
                1, // hardcoded communityid for now
                2,  // pintype for warning
                userId
            );

            if (response == null)
            {
                Console.WriteLine("CreatePin returned null response.");
                await DisplayAlert("Error", "An error occurred while adding the pin.", "OK");
                return;
            }

            Console.WriteLine($"CreatePin response: IsSuccess={response.IsSuccess}, StatusCode={response.StatusCode}, ErrorMessage='{response.ErrorMessage}'");

            if (!response.IsSuccess)
            {
                string message = response.StatusCode == 401
                    ? "You are not authorized to add a pin."
                    : $"An error occurred: {response.ErrorMessage}";

                await DisplayAlert("Error", message, "OK");
            }
            else
            {
                await DisplayAlert("Success", "Pin added successfully \u2714", "OK");
                await Navigation.PopAsync();
            }
        }
        catch (MySqlException mySqlEx)
        {
            string detailedError = mySqlEx.ToString();
            Console.WriteLine("MySQL Exception: " + detailedError);
            await DisplayAlert("Error", "MySQL error:\n" + detailedError, "OK");
        }
        catch (Exception ex)
        {
            string detailedError = ex.ToString();
            Console.WriteLine("General Exception: " + detailedError);
            await DisplayAlert("Error", "An error occurred:\n" + detailedError, "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        NavigationPage.SetHasNavigationBar(this, false);
    }
}
