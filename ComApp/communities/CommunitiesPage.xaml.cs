using comApp;
using comApp.communities;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace comApp.communities;
public partial class CommunitiesPage : ContentPage
{
    private List<Community> _communities = new();

    public CommunitiesPage()
    {
        InitializeComponent();
        LoadCommunities();
    }

    private async void LoadCommunities()
    {
        var response = await GetAllCommunities();

        if (response.IsSuccessStatusCode)
        {
            var json = response.Content;

            try
            {
                var communities = JsonSerializer.Deserialize<List<Community>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (communities != null)
                {
                    _communities = communities;
                    CommunitiesListView.ItemsSource = _communities;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load communities: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", $"API call failed: {response.ErrorMessage}", "OK");
        }
    }

    private async void OnRequestClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Community selectedCommunity)
        {
            string userId = App.UserId; 
            var response = await RequestMembership(userId, selectedCommunity.id);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Success", $"Membership request sent to {selectedCommunity.name}", "OK");
            }
            else
            {
                await DisplayAlert("Error", $"Failed to send request: {response.ErrorMessage}", "OK");
            }
        }
    }

    public Task<ApiResponse> GetAllCommunities()
    {
        return ApiService.GetRequest("/Community");
    }

    public Task<ApiResponse> RequestMembership(string userId, int communityId)
    {
        return ApiService.PutRequest($"/Community/RequestMembership?userId={userId}&communityId={communityId}", null);
    }
}
