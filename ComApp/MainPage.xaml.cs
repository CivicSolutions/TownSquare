using comApp.db;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace comApp
{
    public partial class MainPage : ContentPage
    {
        private readonly dbConnection _dbConnection;
        public ObservableCollection<CustomPin> Pins { get; } = new();

        public MainPage()
        {
            InitializeComponent();
            _dbConnection = new dbConnection();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = LoadPinsAsync();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async Task LoadPinsAsync()
        {
            const int communityId = 1;

            var response = await _dbConnection.GetAllPins(communityId);
            if (!response.IsSuccess)
            {
                string message = response.StatusCode == 401
                    ? "You are not authorized to view pins."
                    : $"Error loading pins: {response.ErrorMessage}";

                await DisplayAlert("Error", message, "OK");

                if (response.StatusCode == 401)
                {
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                return;
            }

            var pinsFromDB = JsonConvert.DeserializeObject<List<PinData>>(response.Content);
            if (pinsFromDB is null) return;

            Console.WriteLine($"Fetched Pins: {JsonConvert.SerializeObject(pinsFromDB)}");

            Pins.Clear();

            foreach (var pin in pinsFromDB)
            {
                if (IsValidCoordinate(pin.XCoord, pin.YCoord))
                {
                    Pins.Add(new CustomPin
                    {
                        Label = pin.Title,
                        Address = pin.Description,
                        Location = new Location(pin.XCoord, pin.YCoord),
                        PinType = pin.PinType
                    });
                }
            }

            Pins.Add(new CustomPin
            {
                Label = "Dummy Pin",
                Address = "Ins, Bern, Switzerland",
                Location = new Location(47.006, 7.106),
                PinType = 2
            });

            map.Pins.Clear();
            foreach (var pin in Pins)
            {
                map.Pins.Add(pin);
            }
        }

        private static bool IsValidCoordinate(double lat, double lon)
        {
            return lat >= -90 && lat <= 90 && lon >= -180 && lon <= 180 && (lat != 0 || lon != 0);
        }

        public class PinData
        {
            public string Title { get; set; } = "";
            public string Description { get; set; } = "";

            [JsonProperty("xCord")]
            public double XCoord { get; set; }

            [JsonProperty("yCord")]
            public double YCoord { get; set; }

            [JsonProperty("pintype")]
            public int PinType { get; set; }
        }

        public class CustomPin : Pin
        {
            public int PinType { get; set; }
        }
    }
}
