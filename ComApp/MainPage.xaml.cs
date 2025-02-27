using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using comApp.db;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;

namespace comApp
{
    public partial class MainPage : ContentPage
    {
        private dbConnection _dbConnection;
        private ObservableCollection<Pin> _pins;
        private CancellationTokenSource _cancellationTokenSource;
        private string _lastResponse = string.Empty;

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
            try
            {
                string response = await _dbConnection.GetPins();
                if (response == _lastResponse)
                {
                    return;
                }

                _lastResponse = response;

                var pinsFromDB = JsonConvert.DeserializeObject<List<PinData>>(response);
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

                    map.ItemsSource = null; // Force refresh
                    map.ItemsSource = Pins;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load map pins: " + ex.Message, "OK");
            }
        }

        private async Task RefreshPinsLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                LoadPins();
                await Task.Delay(TimeSpan.FromSeconds(60), token);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckUser();
            NavigationPage.SetHasNavigationBar(this, false);

            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => RefreshPinsLoop(_cancellationTokenSource.Token));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _cancellationTokenSource?.Cancel();
        }

        private async void CheckUser()
        {
            int userId = App.UserId;
            if (userId < 0)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
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
