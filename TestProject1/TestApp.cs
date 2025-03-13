using Microsoft.Maui.Controls;

namespace TestProject1
{
    public class TestApp : Application
    {
        public TestApp()
        {
            // Avoid null reference errors by initializing Resources
            Resources = new ResourceDictionary();
            MainPage = new ContentPage(); // Placeholder page
        }
    }
}