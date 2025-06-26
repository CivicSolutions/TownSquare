using comApp.db;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Globalization;

namespace comApp.posts
{
    public partial class PostsPage : ContentPage
    {
        private dbConnection _dbConnection;
        private ObservableCollection<Post> _newsPosts;
        private ObservableCollection<Post> _userPosts;
        private string userId;

        public PostsPage()
        {
            InitializeComponent();
            _dbConnection = new dbConnection();
            _newsPosts = new ObservableCollection<Post>();
            _userPosts = new ObservableCollection<Post>();
            userId = App.UserId;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPosts(); // Unified function
            NavigationPage.SetHasNavigationBar(this, false);
        }


        private async Task LoadPosts()
        {
            try
            {
                var response = await _dbConnection.GetAllPosts(userId);

                if (!response.IsSuccess)
                {
                    string message = response.StatusCode == 401
                        ? "You are not authorized to view posts."
                        : $"Failed to load posts: {response.ErrorMessage}";
                    await DisplayAlert("Error", message, "OK");
                    return;
                }

                var allPosts = JsonConvert.DeserializeObject<List<Post>>(response.Content);
                _newsPosts.Clear();
                _userPosts.Clear();

                if (allPosts != null)
                {
                    foreach (var post in allPosts)
                    {
                        if (post.IsNews == 1)
                        {
                            _newsPosts.Add(post); // Admin posts
                        }
                        else
                        {
                            _userPosts.Add(post); // Regular user posts
                        }
                    }
                }

                NewsPostsCollectionView.ItemsSource = _newsPosts;
                UserPostsCollectionView.ItemsSource = _userPosts;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load posts: {ex.Message}", "OK");
            }
        }

        private async void OnCreatePostClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePostPage());
        }

        private async void OnLikeButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Post post)
            {
                try
                {
                    ApiResponse result;
                    if (post.IsLikedByCurrentUser)
                    {
                        result = await _dbConnection.UnlikePost(post.Id, userId);
                    }
                    else
                    {
                        result = await _dbConnection.LikePost(post.Id, userId);
                    }

                    if (result.IsSuccess)
                    {
                        post.IsLikedByCurrentUser = !post.IsLikedByCurrentUser;
                        post.LikeCount += post.IsLikedByCurrentUser ? 1 : -1;
                        OnPropertyChanged(nameof(post.LikeIcon));
                        OnPropertyChanged(nameof(post.LikeBackgroundColor));
                        // Refresh UI
                        NewsPostsCollectionView.ItemsSource = null;
                        UserPostsCollectionView.ItemsSource = null;
                        NewsPostsCollectionView.ItemsSource = _newsPosts;
                        UserPostsCollectionView.ItemsSource = _userPosts;
                    }
                    else
                    {
                        await DisplayAlert("Error", "Could not update like status.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }

        private async Task CheckUser()
        {
            try
            {
                string userId = App.UserId;
                if (userId is null or "")
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to check user session: {ex.Message}", "OK");
            }
        }
        
        private async void OnPostTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is Post post)
            {
                var profileModal = new comApp.profileModal.ProfileModalPage(post.UserId);
                await Navigation.PushModalAsync(profileModal);
            }
        }


    }
}
