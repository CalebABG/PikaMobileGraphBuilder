using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using CS253ShortestPath.Contracts;
using CS253ShortestPath.Models;
using Exception = System.Exception;
using Object = Java.Lang.Object;

namespace CS253ShortestPath.Droid
{
    public class LocationManagerDroid : Object, ILocation, ILocationListener
    {
        // The minimum distance to change updates in meters
        private const float MinDistanceChangeForUpdates = 1.0f;

        // The minimum time between updates in milliseconds
        private const long MinTimeBwUpdates = 1000 * 2;

        // Declaring a Location Manager
        private readonly LocationManager _locationManager;

        public LocationManagerDroid()
        {
            _locationManager = (Application.Context.GetSystemService(Context.LocationService) as LocationManager)!;
        }

        public bool IsNetworkEnabled => _locationManager?.IsProviderEnabled(LocationManager.NetworkProvider) ?? false;
        public bool IsGpsEnabled => _locationManager?.IsProviderEnabled(LocationManager.GpsProvider) ?? false;

        public RoutePoint? CurrentLocation
        {
            get
            {
                try
                {
                    var requestedLocation =
                        _locationManager.GetLastKnownLocation(IsGpsEnabled
                            ? LocationManager.GpsProvider
                            : LocationManager.NetworkProvider);

                    if (requestedLocation == null) return null;

                    return new RoutePoint
                    {
                        Latitude = requestedLocation.Latitude,
                        Longitude = requestedLocation.Longitude
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine($"LocationManagerDroid:CurrentLocation - Exception -- {e.Message}");
                }

                return null;
            }
        }

        public void StartRequestingLocation()
        {
            try
            {
                // First get location from Network Provider
                if (IsNetworkEnabled)
                    _locationManager.RequestLocationUpdates(
                        LocationManager.NetworkProvider,
                        MinTimeBwUpdates,
                        MinDistanceChangeForUpdates, this);

                // if GPS Enabled get lat/long using GPS Services
                if (IsGpsEnabled)
                    _locationManager.RequestLocationUpdates(
                        LocationManager.GpsProvider,
                        MinTimeBwUpdates,
                        MinDistanceChangeForUpdates, this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void StopRequestingLocation()
        {
            _locationManager?.RemoveUpdates(this);
        }

        public void OnLocationChanged(Location location)
        {
            if (location == null) return;

            // do something
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
        }
    }
}