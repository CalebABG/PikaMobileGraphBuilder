using CoreLocation;
using CS253ShortestPath.Contracts;
using CS253ShortestPath.Models;

namespace CS253ShortestPath.iOS
{
    public class LocationManagerIos : ILocation
    {
        private const double DesiredAccuracy = 2.0; // meters
        private const double DistanceBetweenUpdates = 1.0; //meters
        private const double HeadingChangesBetweenUpdates = 7.0; // degrees
        private readonly CLLocationManager _locationManager;


        public LocationManagerIos()
        {
            _locationManager = new CLLocationManager
            {
                PausesLocationUpdatesAutomatically = false,
                ShowsBackgroundLocationIndicator = true,
                DesiredAccuracy = DesiredAccuracy,
                DistanceFilter = DistanceBetweenUpdates,
                HeadingFilter = HeadingChangesBetweenUpdates,
                Delegate = new LocationManagerIosDelegate(this)
            };
        }

        public RoutePoint? CurrentLocation
        {
            get
            {
                var requestedClLocation = _locationManager.Location;
                if (requestedClLocation == null) return null;

                return new RoutePoint
                {
                    Latitude = requestedClLocation.Coordinate.Latitude,
                    Longitude = requestedClLocation.Coordinate.Longitude
                };
            }
        }

        public void StartRequestingLocation()
        {
            _locationManager.StartUpdatingLocation();
        }

        public void StopRequestingLocation()
        {
            _locationManager.StopUpdatingLocation();
        }
    }
}