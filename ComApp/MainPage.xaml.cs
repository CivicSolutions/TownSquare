using comApp.db;
using Microsoft.Maui.Controls.Maps;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace comApp
{
    public partial class MainPage : ContentPage
    {
        private dbConnection _dbConnection;
        private ObservableCollection<Pin> _pins;

        public List<Pin> Pins { get; set; } = new List<Pin>();

        public MainPage()
        {
            InitializeComponent();
            _dbConnection = new dbConnection();
            _pins = new ObservableCollection<Pin>();
            BindingContext = this;
            LoadPins();
        }

        private async void LoadPins()
        {
            int communityId = 1; // TODO: Replace with actual community ID logic
            var response = await _dbConnection.GetAllPins(communityId);

            if (!response.IsSuccess)
            {
                // Show error message (e.g., unauthorized or other errors)  
                string message = response.StatusCode == 401
                    ? "You are not authorized to view pins."
                    : $"Error loading pins: {response.ErrorMessage}";

                await DisplayAlert("Error", message, "OK");

                if (response.StatusCode == 401)
                {
                    // Redirect to login page and hide navigation  
                    await Shell.Current.GoToAsync("//LoginPage");
                    NavigationPage.SetHasNavigationBar(this, false);
                }

                return;
            }

            var pinsFromDB = JsonConvert.DeserializeObject<List<PinData>>(response.Content);

            if (pinsFromDB != null)
            {
                _pins.Clear();
                Pins.Clear();

                foreach (var pin in pinsFromDB)
                {
                    Pins.Add(new Pin
                    {
                        Label = pin.Title,
                        Address = pin.Description,
                        Location = new Location(pin.XCoord, pin.YCoord)
                    });
                }

                map.ItemsSource = Pins;
            }
        }



        private async void CheckUser()
        {
            // Implement a session/user validation method in dbConnection
            int userId = 1; // Example: Replace with actual session retrieval logic

            if (userId < 0)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        private void ReloadPins()
        {
            _pins.Clear();
            LoadPins();
        }

        protected override void OnAppearing()
        {
            ReloadPins();
            base.OnAppearing();
            CheckUser();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public class PinData
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public double XCoord { get; set; }
            public double YCoord { get; set; }
        }
    }
}
