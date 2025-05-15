namespace comApp.posts;
using comApp.db;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

public partial class HelpPostsPage : ContentPage
{
    private dbConnection _dbConnection;
    private ObservableCollection<HelpPosts> _helpposts;

    public HelpPostsPage()
    {
        InitializeComponent();
        _dbConnection = new dbConnection();
        _helpposts = new ObservableCollection<HelpPosts>();
        LoadPosts();
        // CheckUser();
    }

    private async void LoadPosts()
    {
        string response = await _dbConnection.GetHelpPosts();
        var helpPostsFromDb = JsonConvert.DeserializeObject<List<HelpPosts>>(response);

        if (helpPostsFromDb != null)
        {
            _helpposts.Clear();
            foreach (var helppost in helpPostsFromDb)
            {
                _helpposts.Add(helppost);
            }
        }
        HelpPostsCollectionView.ItemsSource = _helpposts;
    }

    private async void OnCreatePostClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPost());
    }

    //private async void CheckUser()
    //{
    //    int userId = await GetUserIdFromSession();

    //    if (userId < 0)
    //    {
    //        await Shell.Current.GoToAsync("//LoginPage");
    //    }
    //}

    //private async Task<int> GetUserIdFromSession()
    //{
    //    string response = await _dbConnection.GetUserById(userId: App.UserId);
    //    if (int.TryParse(response, out int userId))
    //    {
    //        return userId;
    //    }
    //    return -1;
    //}

    private async void OnAcceptButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var selectedHelpPost = (HelpPosts)button.BindingContext;

        int loggedInUserId = App.UserId;

        if (selectedHelpPost.UserId == loggedInUserId)
        {
            await DisplayAlert("Error", "You cannot accept your own post.", "OK");
            return;
        }

        string popupMessage = $"Title: {selectedHelpPost.Title}\n" +
                              $"Description: {selectedHelpPost.Description}\n" +
                              $"Price: {selectedHelpPost.Price} CHF\n" +
                              $"Telephone: {selectedHelpPost.Telephone}\n\n" +
                              "Are you sure you want to accept this post?";

        bool accept = await DisplayAlert("Confirm Acceptance", popupMessage, "Accept", "Cancel");

        if (accept)
        {
            // await _dbConnection.DeletePost($"/HelpPost/DeleteHelpPost/{selectedHelpPost.Id}");
            LoadPosts();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // CheckUser();
        NavigationPage.SetHasNavigationBar(this, false);
    }
}
