namespace comApp.posts;
using comApp.db;
using comApp.login;
using System.Collections.ObjectModel;

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
        CheckUser();
    }
    private void LoadPosts()
    {
        // Fetch posts from the database
        // You need to implement a method in dbConnection to fetch posts
        // Assuming you have a method GetPosts() which returns a list of Post objects
        var helpPostsFromDb = _dbConnection.GetHelpPosts();

        _helpposts.Clear(); // Clear existing posts
        foreach (var helppost in helpPostsFromDb)
        {
            _helpposts.Add(helppost);
        }
        HelpPostsCollectionView.ItemsSource = _helpposts;
    }

    private async void OnCreatePostClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HelpPost());
    }
    private async void CheckUser()
    {
        int userId = _dbConnection.GetUserIdFromSession();

        if (userId < 0)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
    private async void OnAcceptButtonClicked(object sender, EventArgs e)
    {
        // Get the selected help post from the event arguments
        var button = (Button)sender;
        var selectedHelpPost = (HelpPosts)button.BindingContext;

        // Get the ID of the logged-in user
        int loggedInUserId = _dbConnection.GetUserIdFromSession();

        // Check if the logged-in user is the same as the user who created the post
        if (selectedHelpPost.UserId == loggedInUserId)
        {
            await DisplayAlert("Error", "You cannot accept your own post.", "OK");
            return; // Exit the method
        }

        // Display a confirmation popup with post details
        string popupMessage = $"Title: {selectedHelpPost.Title}\n" +
                              $"Description: {selectedHelpPost.Description}\n" +
                              $"Price: {selectedHelpPost.Price} CHF\n" +
                              $"Telephone: {selectedHelpPost.Telephone}\n\n" +
                              "Are you sure you want to accept this post?";

        bool accept = await DisplayAlert("Confirm Acceptance", popupMessage, "Accept", "Cancel");

        if (accept)
        {
            // Delete the help post from the database
            _dbConnection.DeleteHelpPost(selectedHelpPost.Id);

            // Refresh the collection view
            LoadPosts();
        }
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        CheckUser();
        NavigationPage.SetHasNavigationBar(this, false);
    }
}