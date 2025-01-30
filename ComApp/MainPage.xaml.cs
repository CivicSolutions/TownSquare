
using comApp.db; // Assuming this is the namespace of your db class
using Microsoft.Maui.Controls.Maps;
using comApp.login;
using Microsoft.Maui.Maps;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace comApp
{
    public partial class MainPage : ContentPage
    {
        private dbConnection _dbConnection;
        private ObservableCollection<Pin> _pins;

        int count = 0;

        public List<Pin> Pins {  get; set; } = new List<Pin>();

        public MainPage()
        {
            InitializeComponent();
            _dbConnection = new dbConnection();
            _pins = new ObservableCollection<Pin>();
            BindingContext = this;
            LoadPins();
        }


      private void LoadPins()
        {

            var pinsFromDB = _dbConnection.GetPins();

            _pins.Clear();
            foreach (var pin in pinsFromDB)
            {

                Pins.Add(new Pin
                {
                    Label = pin.Title,
                    Address = pin.Description,
                    Location = new Location(pin.XCoord, pin.YCoord)
                }) ;
            }
            map.ItemsSource = Pins;
            
        }

        private async void CheckUser()
        {
            int userId = _dbConnection.GetUserIdFromSession();

            if (userId < 0)
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
         


        private void TestDatabaseConnection()
        {
            dbConnection database = new dbConnection();
            if (database.TestConnection())
            {
                DisplayAlert("Success", "Connection to MySQL database successful!", "OK");
            }
            else
            {
                DisplayAlert("Error", "Connection to MySQL database failed.", "OK");
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


        //protected override void OnMapReady(Map map)
        //{

        //    // Add custom overlay or styling here
        //    // Example: Draw a polygon outline around a specific area
        //    Polygon polygonOptions = new Polygon {
        //        StrokeWidth = 8,
        //        StrokeColor = Color.FromArgb("#1BA1E2"),
        //        FillColor = Color.FromArgb("#881BA1E2"),
        //        Geopath = {
        //        new Location(47.01076501796103, 7.10304499774112),
        //         new Location(47.010569568821865, 7.110334150593581),
        //         new Location(47.00705771866243, 7.11415536689692),
        //         new Location(47.00149596055266, 7.108850525371189),
        //         new Location(46.99888714881887, 7.110041408161284),
        //         new Location(46.99866564005672, 7.104123081456586),
        //         new Location(46.99992084414791, 7.09744692028574),
        //         new Location(47.00806664807105, 7.093874271870474)
        //    }
        //};

    }
}
