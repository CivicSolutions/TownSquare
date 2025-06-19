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
            BindingContext = userData;

            string imageUrl = userData?.profileImage;
            ProfileImage.Source = string.IsNullOrEmpty(imageUrl)
                ? "default_profile.png" // 👈 Replace with local placeholder image
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
            string password = current.password;

            var updateResponse = await _dbConnection.UpdateUser(userId, email, password, usernameEntry.Text, bioEditor.Text);
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
                    await DisplayAlert("Warning", "Profile updated, but failed to upload profile image.", "OK");
                    return;
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

        private void OnUsernameEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            usernameErrorLabel.Text = e.NewTextValue.Length > 40
                ? "Username must be maximum 40 characters long"
                : string.Empty;
        }

        private void OnBioEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            bioErrorLabel.Text = e.NewTextValue.Length > 200
                ? "Bio must be maximum 200 characters long"
                : string.Empty;
        }
    }
}
