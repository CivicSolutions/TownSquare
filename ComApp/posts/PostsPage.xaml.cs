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
            await LoadNewsPosts();
            await LoadUserPosts();
            await CheckUser();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async Task LoadNewsPosts()
        {
            try
            {
                string response = await _dbConnection.GetCommunityPosts(1);
                var newsPostsFromApi = JsonConvert.DeserializeObject<List<Post>>(response);
                _newsPosts.Clear();
                foreach (var newsPost in newsPostsFromApi)
                {
                    _newsPosts.Add(newsPost);
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
                string response = await _dbConnection.GetCommunityPosts(0);
                var userPostsFromApi = JsonConvert.DeserializeObject<List<Post>>(response);
                _userPosts.Clear();
                foreach (var userPost in userPostsFromApi)
                {
                    _userPosts.Add(userPost);
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
                int userId = Preferences.Get("userId", -1);
                if (userId < 0)
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
