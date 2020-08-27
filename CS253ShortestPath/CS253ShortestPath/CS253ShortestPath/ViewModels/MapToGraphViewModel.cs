using System;
using CS253ShortestPath.Models;
using CS253ShortestPath.Pages;
using MvvmHelpers;

namespace CS253ShortestPath.ViewModels
{
    public class MapToGraphViewModel : CsViewModelBase
    {
        private bool _autoAddWayPoints;
        private bool _canCollapse = true;

        private int _markerDropInterval = 3;

        public ObservableRangeCollection<RoutePoint> CurrentRoutePoints { get; set; }
            = new ObservableRangeCollection<RoutePoint>();

        public int MarkerDropInterval
        {
            get => _markerDropInterval;
            set => Set(ref _markerDropInterval, value);
        }

        public bool AutoAddWayPoints
        {
            get => _autoAddWayPoints;
            set
            {
                if (Set(ref _autoAddWayPoints, value))
                {
                    RaisePropertyChanged(nameof(AddWayPointsText));

                    // If value is now true, manually trigger adding a Marker based on Time
                    if (_autoAddWayPoints)
                        MapToGraph.NextTime = DateTimeOffset.Now.AddSeconds(-_markerDropInterval);
                }
            }
        }

        public string AddWayPointsText => AutoAddWayPoints ? "Auto Add On" : "Auto Add Off";

        public bool CanCollapse
        {
            get => _canCollapse;
            set => Set(ref _canCollapse, value);
        }
    }
}