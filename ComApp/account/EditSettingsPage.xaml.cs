using comApp.db;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;
using System;

namespace comApp.account
{
    public partial class EditSettingsPage : ContentPage
    {
        private dbConnection _dbConnection;
        private FileResult _newProfileImage;

        public EditSettingsPage()
        {
            InitializeComponent();

            firstNameEntry.TextChanged += OnFirstNameChanged;
            lastNameEntry.TextChanged += OnLastNameChanged;
            bioEditor.TextChanged += OnBioEditorChanged;

            ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = "close_icon.png",
                Priority = 0,
                Order = ToolbarItemOrder.Primary,
                Command = new Command(OnCloseClicked)
            });

            _dbConnection = new dbConnection();
            LoadUser();
        }

        private async void LoadUser()
        {
            var response = await _dbConnection.GetUserById(App.UserId);

            if (!response.IsSuccess)
            {
                await DisplayAlert("Error", "Failed to load user data.", "OK");
                return;
            }

            dynamic userData = JsonConvert.DeserializeObject(response.Content);

            firstNameEntry.Text = userData?.firstName;
            lastNameEntry.Text = userData?.lastName;
            bioEditor.Text = userData?.description;

            string imageUrl = userData?.profileImage;
            ProfileImage.Source = string.IsNullOrEmpty(imageUrl)
                ? "default_profile.png"
                : ImageSource.FromUri(new Uri(imageUrl));
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            string userId = App.UserId;

            var currentResponse = await _dbConnection.GetUserById(userId);
            if (!currentResponse.IsSuccess)
            {
                await DisplayAlert("Error", "Failed to get current user data.", "OK");
                return;
            }

            dynamic current = JsonConvert.DeserializeObject(currentResponse.Content);
            string email = current.email;

            string firstName = firstNameEntry.Text?.Trim();
            string lastName = lastNameEntry.Text?.Trim();
            string description = bioEditor.Text?.Trim();

            var updateResponse = await _dbConnection.UpdateUser(userId, email, firstName, lastName, description);
            if (!updateResponse.IsSuccess)
            {
                await DisplayAlert("Error", "Failed to update user data.", "OK");
                return;
            }

            if (_newProfileImage != null)
            {
                var imageResponse = await _dbConnection.UploadUserImage(userId, _newProfileImage);
                if (!imageResponse.IsSuccess)
                {
                    await DisplayAlert("Warning", "Profile updated, but image upload failed.", "OK");
                }
            }

            await DisplayAlert("Success", "Profile updated successfully.", "OK");
            await Navigation.PopAsync();
        }

        private async void OnChangeProfilePictureClicked(object sender, EventArgs e)
        {
            try
            {
                _newProfileImage = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Choose a profile picture"
                });

                if (_newProfileImage != null)
                {
                    using var stream = await _newProfileImage.OpenReadAsync();
                    ProfileImage.Source = ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
            }
        }

        private async void OnCloseClicked()
        {
            bool result = await DisplayAlert("Close", "Are you sure you want to close?", "Yes", "No");
            if (result)
                await Navigation.PopAsync();
        }

        private void OnFirstNameChanged(object sender, TextChangedEventArgs e)
        {
            firstNameErrorLabel.Text = e.NewTextValue.Length > 40
                ? "First name must be maximum 40 characters long"
                : string.Empty;
        }

        private void OnLastNameChanged(object sender, TextChangedEventArgs e)
        {
            lastNameErrorLabel.Text = e.NewTextValue.Length > 40
                ? "Last name must be maximum 40 characters long"
                : string.Empty;
        }

        private void OnBioEditorChanged(object sender, TextChangedEventArgs e)
        {
            bioErrorLabel.Text = e.NewTextValue.Length > 200
                ? "Bio must be maximum 200 characters long"
                : string.Empty;
        }
    }
}
