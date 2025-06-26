using Microsoft.Maui.Controls;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using comApp.db;
using Newtonsoft.Json;
using static comApp.posts.HelpPostsPage;

namespace comApp.posts
{
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
                await Navigation.PopAsync();
        }

        protected void OnTitleEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                titleErrorLabel.Text = "Title cannot be empty";
            else if (e.NewTextValue.Length > 50)
                titleErrorLabel.Text = "Title must be 50 characters max";
            else
                titleErrorLabel.Text = string.Empty;
        }

        protected void OnDescriptionEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                descriptionErrorLabel.Text = "Description cannot be empty";
            else if (e.NewTextValue.Length > 300)
                descriptionErrorLabel.Text = "Description must be 300 characters max";
            else
                descriptionErrorLabel.Text = string.Empty;
        }

        protected void OnPriceEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue) || !int.TryParse(e.NewTextValue, out _))
                priceErrorLabel.Text = "Price must be a number (e.g., 20)";
            else
                priceErrorLabel.Text = string.Empty;
        }

        protected void OnTelephoneEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            string tel = e.NewTextValue?.Replace(" ", "") ?? "";
            if (!Regex.IsMatch(tel, @"^0\d{9}$"))
                telephoneErrorLabel.Text = "Invalid Swiss phone number format (e.g., 0760000000)";
            else
                telephoneErrorLabel.Text = string.Empty;
        }

        private async void OnSubmitPostClicked(object sender, EventArgs e)
        {
            // Validate all fields first
            if (!ValidateForm())
                return;

            string title = titleEntry.Text;
            string description = descriptionEditor.Text;
            double price = double.Parse(priceEntry.Text);
            string telephone = telephoneEntry.Text;
            string userId = App.UserId;
            DateTime postedAt = DateTime.UtcNow;

            var response = await _dbConnection.AddHelpPost(title, description, price, telephone, postedAt, userId);

            if (response != null && response.IsSuccess)
            {
                await DisplayAlert("Success", "Help post submitted!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                string message = response == null
                    ? "There was an issue creating the help post. Please try again."
                    : response.StatusCode == 401
                        ? "You are not authorized to create a help post."
                        : $"There was an issue creating the help post: {response.ErrorMessage}";
                await DisplayAlert("Error", message, "OK");
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(titleEntry.Text) || titleEntry.Text.Length > 50)
            {
                titleErrorLabel.Text = "Title is required (max 50 characters)";
                isValid = false;
            }
            else titleErrorLabel.Text = "";

            if (string.IsNullOrWhiteSpace(descriptionEditor.Text) || descriptionEditor.Text.Length > 300)
            {
                descriptionErrorLabel.Text = "Description is required (max 300 characters)";
                isValid = false;
            }
            else descriptionErrorLabel.Text = "";

            if (string.IsNullOrWhiteSpace(priceEntry.Text) || !int.TryParse(priceEntry.Text, out _))
            {
                priceErrorLabel.Text = "Price must be a valid number";
                isValid = false;
            }
            else priceErrorLabel.Text = "";

            string tel = telephoneEntry.Text?.Replace(" ", "") ?? "";
            if (!Regex.IsMatch(tel, @"^0\d{9}$"))
            {
                telephoneErrorLabel.Text = "Phone number must be in Swiss format (e.g., 0760000000)";
                isValid = false;
            }
            else telephoneErrorLabel.Text = "";

            return isValid;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
