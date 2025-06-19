namespace comApp.posts;
using Microsoft.Maui.Controls;
using System.Text.RegularExpressions;
using comApp.db;

public partial class HelpPost : ContentPage
{
    private dbConnection _dbConnection;
    public HelpPost()
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

    protected void OnTitleEntryTextChanged(object sender, TextChangedEventArgs e)
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

    protected void OnDescriptionEditorTextChanged(object sender, TextChangedEventArgs e)
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

    protected void OnPriceEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        string InputPrice = e.NewTextValue;
        int price;
        int.TryParse(InputPrice, out price);
        if (string.IsNullOrWhiteSpace(InputPrice))
        {
            priceErrorLabel.Text = "Price cannot be empty";
        }
        else if (InputPrice != price.ToString())
        {
            priceErrorLabel.Text = "Price must be a number for example: 20";
        }
        else
        {
            priceErrorLabel.Text = string.Empty;
        }
    }

    protected void OnTelephoneEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        string telephone = e.NewTextValue;
        if (string.IsNullOrWhiteSpace(telephone))
        {
            telephoneErrorLabel.Text = "Telephone number cannot be empty";
        }
        else if (!IsValidSwissPhoneNumber(telephone))
        {
            telephoneErrorLabel.Text = "Invalid Swiss telephone number format (e.g., 076 000 00 00)";
        }
        else
        {
            telephoneErrorLabel.Text = string.Empty;
        }
    }

    private async void OnSubmitPostClicked(object sender, EventArgs e)
    {
        Console.WriteLine("Submit button clicked.");

        if (string.IsNullOrWhiteSpace(titleEntry.Text) || titleEntry.Text.Length > 50)
        {
            Console.WriteLine($"Title validation failed. Value: '{titleEntry.Text}'");
            titleErrorLabel.Text = string.IsNullOrWhiteSpace(titleEntry.Text) ? "Title cannot be empty" : "Title must be maximum 50 characters long";
            return;
        }

        if (string.IsNullOrWhiteSpace(descriptionEditor.Text) || descriptionEditor.Text.Length > 300)
        {
            Console.WriteLine($"Description validation failed. Length: {descriptionEditor.Text?.Length}");
            descriptionErrorLabel.Text = string.IsNullOrWhiteSpace(descriptionEditor.Text) ? "Description cannot be empty" : "Description must be maximum 300 characters long";
            return;
        }

        if (string.IsNullOrWhiteSpace(priceEntry.Text))
        {
            Console.WriteLine("Price is empty.");
            priceErrorLabel.Text = "Price cannot be empty";
            return;
        }

        if (IsValidPrice(priceEntry.Text))
        {
            Console.WriteLine($"Price validation failed. Value: '{priceEntry.Text}'");
            priceErrorLabel.Text = "Price must be a number for example: 20";
            return;
        }

        if (string.IsNullOrWhiteSpace(telephoneEntry.Text))
        {
            Console.WriteLine("Telephone number is empty.");
            telephoneErrorLabel.Text = "Telephone number cannot be empty";
            return;
        }

        if (!IsValidSwissPhoneNumber(telephoneEntry.Text))
        {
            Console.WriteLine($"Telephone number validation failed. Value: '{telephoneEntry.Text}'");
            telephoneErrorLabel.Text = "Invalid Swiss telephone number format (e.g., 076 000 00 00)";
            return;
        }

        string userId = App.UserId;
        string title = titleEntry.Text;
        string content = descriptionEditor.Text;
        string InputPrice = priceEntry.Text;
        int communityId = 1;
        int price;
        int.TryParse(InputPrice, out price);
        string telephone = telephoneEntry.Text;

        Console.WriteLine("Validation passed. Preparing to submit post:");
        Console.WriteLine($"UserId: {userId}, Title: {title}, Description: {content}, Price: {price}, Telephone: {telephone}, CommunityId: {communityId}");

        var response = await _dbConnection.AddHelpPost(title, content, communityId, price, telephone, userId);

        if (response != null && response.IsSuccess)
        {
            Console.WriteLine("Post submission succeeded.");
            await Navigation.PushAsync(new HelpPostsPage());
        }
        else
        {
            Console.WriteLine("Post submission failed.");
            if (response == null)
            {
                Console.WriteLine("Response is null.");
            }
            else
            {
                Console.WriteLine($"StatusCode: {response.StatusCode}, ErrorMessage: {response.ErrorMessage}");
            }

            string message = response == null
                ? "There was an issue creating the help post. Please try again."
                : response.StatusCode == 401
                    ? "You are not authorized to create a help post."
                    : $"There was an issue creating the help post: {response.ErrorMessage}";
            await DisplayAlert("Error", message, "OK");
        }
    }


    private bool IsValidSwissPhoneNumber(string phoneNumber)
    {
        string normalized = phoneNumber.Replace(" ", "");
        return Regex.IsMatch(normalized, @"^\d{10}$");
    }

    private bool IsValidPrice(string InputPrice)
    {
        int price;
        int.TryParse(InputPrice, out price);
        if (InputPrice != price.ToString())
        {
            return true;
        }
        else { return false; }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        NavigationPage.SetHasNavigationBar(this, false);
    }
}
