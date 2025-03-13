using Microsoft.Maui.Controls;

namespace comApp
{
    public partial class App : Application
    {
        public static int UserId { get; set; }

        public App()
        {
            InitializeComponent();

            if (!IsRunningTest())
            {
                MainPage = new AppShell();
            }
        }

// Add this helper method
        private static bool IsRunningTest()
        {
            return AppDomain.CurrentDomain.FriendlyName.Contains("test", StringComparison.OrdinalIgnoreCase);
        }

    }
}