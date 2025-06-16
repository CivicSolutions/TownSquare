using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using System.Collections.Specialized;
using static comApp.MainPage;

namespace comApp.Platforms.Android
{
    public class CustomMapHandler : MapHandler
    {
        private GoogleMap? _googleMap;

        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);

            platformView.GetMapAsync(new OnMapReadyCallback(map =>
            {
                _googleMap = map;

                map.MarkerClick += (sender, e) =>
                {
                    e.Handled = false;
                };

                UpdatePins();

                // Listen for dynamic changes to Pins collection
                if (VirtualView?.Pins is INotifyCollectionChanged observablePins)
                {
                    observablePins.CollectionChanged += (s, e) => UpdatePins();
                }
            }));
        }

        private void UpdatePins()
        {
            if (_googleMap == null || VirtualView == null)
                return;

            _googleMap.Clear();

            foreach (var pin in VirtualView.Pins)
            {
                var customPin = pin as CustomPin;

                var marker = new MarkerOptions()
                    .SetPosition(new LatLng(pin.Location.Latitude, pin.Location.Longitude))
                    .SetTitle(pin.Label)
                    .SetSnippet(pin.Address);

                if (customPin != null)
                {
                    switch (customPin.PinType)
                    {
                        case 1:
                            marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueYellow));
                            break;
                        case 2:
                            marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
                            break;
                        default:
                            marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueAzure));
                            break;
                    }
                }

                _googleMap.AddMarker(marker);
            }
        }

        private class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
        {
            private readonly Action<GoogleMap> _onMapReady;

            public OnMapReadyCallback(Action<GoogleMap> onMapReady)
            {
                _onMapReady = onMapReady;
            }

            public void OnMapReady(GoogleMap googleMap)
            {
                _onMapReady?.Invoke(googleMap);
            }
        }
    }
}
