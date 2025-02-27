using comApp.db;

namespace comApp.posts
{
    public partial class CreatePostPage : ContentPage
    {
        private dbConnection _dbConnection;

        public CreatePostPage()
        {
            InitializeComponent();
            _dbConnection = new dbConnection();

            ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = "close_icon.png",
                Priority = 0,
                Order = ToolbarItemOrder.Primary,
                Command = new Command(OnCloseClicked)
            });
        }

        private async void OnCloseClicked()
        {
            bool result = await DisplayAlert("Close", "Are you sure you want to close?", "Yes", "No");

            if (result)
            {
                await Navigation.PopAsync();
            }
        }

        protected void OnTitleEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            string title = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(title))
            {
                titleErrorLabel.Text = "Title cannot be empty";
            }
            else if (title.Length > 100)
            {
                titleErrorLabel.Text = "Title must be maximum 100 characters long";
            }
            else
            {
                titleErrorLabel.Text = string.Empty;
            }
        }

        protected void OnContentEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            string content = e.NewTextValue;
            if (content.Length < 50)
            {
                contentErrorLabel.Text = "Content must be at least 50 characters long";
            }
            else if (content.Length > 600)
            {
                contentErrorLabel.Text = "Content must be maximum 600 characters long";
            }
            else
            {
                contentErrorLabel.Text = string.Empty;
            }
        }

        private async void OnSubmitPostClicked(object sender, EventArgs e)
        {
            string title = titleEntry.Text; // Corrected from TitleEntry
            string content = contentEditor.Text; // Corrected from ContentEntry

            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content) && content.Length >= 50 && content.Length <= 600)
            {
                // Assuming the userId is available through some mechanism (e.g., a logged-in user)
                int userId = App.UserId; // Replace with actual user ID

                string response = await _dbConnection.CreatePost(userId, content, 0, 0); // Adjust parameters accordingly
                if (response.Contains("Error"))
                {
                    await DisplayAlert("Error", "Failed to add post", "OK");
                }
                else
                {
                    DisplayAlert("Success", "Post added successfully", "OK");
                    Navigation.PopAsync();
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(title) || title.Length > 100)
                {
                    titleErrorLabel.Text = string.IsNullOrWhiteSpace(title) ? "Title cannot be empty" : "Title must be maximum 100 characters long";
                }
                else
                {
                    titleErrorLabel.Text = string.Empty;
                }

                if (string.IsNullOrWhiteSpace(content) || content.Length < 50)
                {
                    contentErrorLabel.Text = "Content must be at least 50 characters long";
                }
                else if (content.Length > 600)
                {
                    contentErrorLabel.Text = "Content must be maximum 600 characters long";
                }
                else
                {
                    contentErrorLabel.Text = string.Empty;
                }
            }
        }
    }
}
