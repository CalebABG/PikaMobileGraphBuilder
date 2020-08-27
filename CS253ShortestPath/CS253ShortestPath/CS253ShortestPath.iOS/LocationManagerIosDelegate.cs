using System;
using System.Linq;
using CoreLocation;
using Foundation;

namespace CS253ShortestPath.iOS
{
    public class LocationManagerIosDelegate : CLLocationManagerDelegate
    {
        private readonly WeakReference<LocationManagerIos> _locationManagerIos;

        public LocationManagerIosDelegate(LocationManagerIos locationManagerIos)
        {
            _locationManagerIos = new WeakReference<LocationManagerIos>(locationManagerIos);
        }

        private LocationManagerIos LocationManagerIos
        {
            get
            {
                _locationManagerIos.TryGetTarget(out var locationManager);
                return locationManager;
            }
        }

        public override void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
        {
            Console.WriteLine($"LocationManagerIosDelegate:AuthorizationChanged - Status: {status}");
        }

        public override void Failed(CLLocationManager manager, NSError error)
        {
            Console.WriteLine($"LocationManagerIosDelegate:Failed - Error: {error}");
        }

        public override void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            var location = locations.Last();

            if (location == null) return;

            // do something
        }
    }
}