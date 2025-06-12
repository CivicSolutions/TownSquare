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
            MainPage = new AppShell();
        }
    }
}