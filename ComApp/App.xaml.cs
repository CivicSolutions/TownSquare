using dotenv.net;

namespace comApp
{
    public partial class App : Application
    {
        public static string UserId { get; set; }
        public static string SessionToken { get; set; }
        public App()
        {
            InitializeComponent();
            LoadSessionToken();
            MainPage = new NavigationPage(new login.Login());
        }

        private async void LoadSessionToken()
        {
            SessionToken = await SecureStorage.GetAsync("session_token");
        }
    }
}