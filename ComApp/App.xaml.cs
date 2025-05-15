namespace comApp
{
    public partial class App : Application
    {
        public static int UserId { get; set; }
        public static string SessionToken { get; set; }
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}