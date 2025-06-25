namespace comApp.profileModal;

using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

public partial class ProfileModalPage : ContentPage
{
    private readonly string _userId;

    public ProfileModalPage(string userId)
    {
        InitializeComponent();
        _userId = userId;
        LoadUser();
    }

    private async void LoadUser()
    {
        string userId = App.UserId;
        var db = new comApp.db.dbConnection();

        var response = await db.GetUserById(userId);
        if (response == null || !response.IsSuccess)
        {
            await DisplayAlert("Error", "Could not load user info.", "OK");
            await CloseModal();
            return;
        }

        dynamic user = JsonConvert.DeserializeObject(response.Content);
        nameLabel.Text = $"{user.firstName} {user.lastName}";

        try
        {
            var imageUrl = $"http://mc.dominikmeister.com/api/ProfilePicture/ImageFile?userId={userId}";
            profileImage.Source = ImageSource.FromUri(new Uri(imageUrl));
            Console.WriteLine($"Loaded image from: {imageUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load profile image: {ex.Message}");
            profileImage.Source = "account_icon.png";
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await CloseModal();
    }

    private async Task CloseModal()
    {
        await Navigation.PopModalAsync();
    }
}