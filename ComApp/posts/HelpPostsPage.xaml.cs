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
        var response = await _dbConnection.GetHelpPosts();

        if (!response.IsSuccess)
        {
            string message = response.StatusCode == 401
                ? "You are not authorized to view help posts."
                : $"Failed to load help posts: {response.ErrorMessage}";
            await DisplayAlert("Error", message, "OK");
            return;
        }

        var helpPostsFromDb = JsonConvert.DeserializeObject<List<HelpPosts>>(response.Content);

        _helpposts.Clear();
        if (helpPostsFromDb != null)
        {
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


    private async void CheckUser()
    {
        string userId = App.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }


    private async void OnAcceptButtonClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var selectedHelpPost = (HelpPosts)button.BindingContext;

        string loggedInUserId = App.UserId;

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
