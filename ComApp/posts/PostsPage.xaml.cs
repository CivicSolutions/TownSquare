using comApp.db;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace comApp.posts
{
    public partial class PostsPage : ContentPage
    {
        private dbConnection _dbConnection;
        private ObservableCollection<Post> _newsPosts;
        private ObservableCollection<Post> _userPosts;

        public PostsPage()
        {
            InitializeComponent();
            _dbConnection = new dbConnection();
            _newsPosts = new ObservableCollection<Post>();
            _userPosts = new ObservableCollection<Post>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckUser();
            await LoadNewsPosts();
            await LoadUserPosts();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async Task LoadNewsPosts()
        {
            try
            {
                var response = await _dbConnection.GetAllPosts(1);

                if (!response.IsSuccess)
                {
                    string message = response.StatusCode == 401
                        ? "You are not authorized to view news posts."
                        : $"Failed to load news posts: {response.ErrorMessage}";
                    await DisplayAlert("Error", message, "OK");
                    return;
                }

                var newsPostsFromApi = JsonConvert.DeserializeObject<List<Post>>(response.Content);
                _newsPosts.Clear();
                if (newsPostsFromApi != null)
                {
                    foreach (var newsPost in newsPostsFromApi)
                    {
                        _newsPosts.Add(newsPost);
                    }
                }
                NewsPostsCollectionView.ItemsSource = _newsPosts;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load news posts: {ex.Message}", "OK");
            }
        }

        private async Task LoadUserPosts()
        {
            try
            {
                var response = await _dbConnection.GetAllPosts(0);

                if (!response.IsSuccess)
                {
                    string message = response.StatusCode == 401
                        ? "You are not authorized to view user posts."
                        : $"Failed to load user posts: {response.ErrorMessage}";
                    await DisplayAlert("Error", message, "OK");
                    return;
                }

                var userPostsFromApi = JsonConvert.DeserializeObject<List<Post>>(response.Content);
                _userPosts.Clear();
                if (userPostsFromApi != null)
                {
                    foreach (var userPost in userPostsFromApi)
                    {
                        _userPosts.Add(userPost);
                    }
                }
                UserPostsCollectionView.ItemsSource = _userPosts;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load user posts: {ex.Message}", "OK");
            }
        }

        private async void OnCreatePostClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePostPage());
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
    }
}
