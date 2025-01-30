using comApp.db;
using System;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

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
            LoadNewsPosts();
            LoadUserPosts();
        }

        private void LoadNewsPosts()
        {
            var newsPostsFromDb = _dbConnection.GetNewsPosts();
            _newsPosts.Clear();
            foreach (var newsPost in newsPostsFromDb)
            {
                _newsPosts.Add(newsPost);
            }
            NewsPostsCollectionView.ItemsSource = _newsPosts;
        }

        private void LoadUserPosts()
        {
            var userPostsFromDb = _dbConnection.GetUserPosts();
            _userPosts.Clear();
            foreach (var userPost in userPostsFromDb)
            {
                _userPosts.Add(userPost);
            }
            UserPostsCollectionView.ItemsSource = _userPosts;
        }

        private async void OnCreatePostClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePostPage());
        }

        private async void CheckUser()
        {
            int userId = _dbConnection.GetUserIdFromSession();
            if (userId < 0)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadNewsPosts();
            LoadUserPosts();
            CheckUser();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
