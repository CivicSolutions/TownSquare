using System.Text.Json;
using comApp.communities;

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
            try
            {
                var communities = JsonSerializer.Deserialize<List<Community>>(response.Content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (communities != null)
                {
                    _communities = communities;

                    foreach (var c in _communities)
                        c.Status = "Loading...";

                    CommunitiesListView.ItemsSource = _communities;
                    string userId = App.UserId; 
                    foreach (var community in _communities)
                    {
                        var status = await ApiService.GetMembershipStatus(community.id, userId);
                        community.Status = status;
                    }

                    CommunitiesListView.ItemsSource = null;
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
                selectedCommunity.Status = "Pending";
                CommunitiesListView.ItemsSource = null;
                CommunitiesListView.ItemsSource = _communities;
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
