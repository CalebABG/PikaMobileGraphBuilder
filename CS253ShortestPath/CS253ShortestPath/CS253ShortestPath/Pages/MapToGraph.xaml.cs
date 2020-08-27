using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CS253ShortestPath.Helpers;
using CS253ShortestPath.Models;
using CS253ShortestPath.Services;
using CS253ShortestPath.ViewModels;
using Newtonsoft.Json;
using Plugin.Toast;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CS253ShortestPath.Pages
{
    public partial class MapToGraph : ContentPage
    {
        public static DateTimeOffset NextTime;
        private bool _didLoadEventHandlers;
        private Timer? _timer;
        private bool _timerStarted;
        private bool _toggleBusy;

        private readonly MapToGraphViewModel _vm;


        public MapToGraph()
        {
            InitializeComponent();

            SubscribeToViewEventHandlers();

            BindingContext = _vm = new MapToGraphViewModel();

            NextTime = DateTimeOffset.Now.AddSeconds(_vm.MarkerDropInterval);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (_vm.CanCollapse)
                _ = TogglePancakeView();
        }

        // Prevent hardware back button on Android
        protected override bool OnBackButtonPressed()
        {
            return true;
        }


        #region Timer Methods

        private void StartTimer()
        {
            if (_timer != null)
                return;

            _timer = new Timer(UpdateTimerCallback, null, 0, 1 * 1000);

            _timerStarted = true;
        }

        private void StopTimer()
        {
            if (_timer == null)
                return;

            _timer.Dispose();
            _timer = null;

            _timerStarted = false;
        }

        private void UpdateTimerCallback(object state)
        {
            if (DateTimeOffset.Now <= NextTime) return;

            if (_vm.AutoAddWayPoints)
                AddRoutePoint();

            NextTime = DateTimeOffset.Now.AddSeconds(_vm.MarkerDropInterval);
        }

        #endregion


        #region Page Helper Methods

        private void AddRoutePoint()
        {
            var p = CsService.Instance.Location.CurrentLocation;
            if (p == null)
            {
                CrossToastPopUp.Current.ShowToastMessage("Requested Location is Null");
                return;
            }

            _vm.CurrentRoutePoints.Add(p);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Todo: Need to fix this, will jump connect points if paused (need to force create new line if auto-add is paused)
                if (MyMap.MapElements.Count < 1)
                {
                    var currentRoutePointsCount = _vm.CurrentRoutePoints.Count;
                    if (currentRoutePointsCount < 2) return;

                    var r1 = _vm.CurrentRoutePoints[currentRoutePointsCount - 2];
                    var r2 = _vm.CurrentRoutePoints[currentRoutePointsCount - 1];

                    MyMap.MapElements.Add(CreatePolylineFromTwoRoutePoints(r1, r2));
                }
                else
                {
                    var lines = MyMap.MapElements.OfType<Polyline>().ToList();
                    if (lines == null || lines.Count < 1) return;

                    var lastLine = lines.Last();
                    lastLine.Geopath.Add(new Position(p.Latitude, p.Longitude));
                }
            });
        }

        private Polyline CreatePolylineFromTwoRoutePoints(RoutePoint routePoint1, RoutePoint routePoint2)
        {
            var polyline = new Polyline
            {
                StrokeColor = Globals.DefaultColor,
                StrokeWidth = Constants.RouteStrokeWidth
            };

            polyline.Geopath.Add(new Position(routePoint1.Latitude, routePoint1.Longitude));
            polyline.Geopath.Add(new Position(routePoint2.Latitude, routePoint2.Longitude));

            return polyline;
        }

        private Polyline CreatePolyline(Route route)
        {
            var polyline = new Polyline
            {
                StrokeColor = Color.FromHex(route.Color),
                StrokeWidth = Constants.RouteStrokeWidth
            };

            var routePoints = route.RoutePoints;
            foreach (var routePoint in routePoints)
                polyline.Geopath.Add(new Position(routePoint.Latitude, routePoint.Longitude));

            return polyline;
        }

        private async Task<int> SaveRoute(List<RoutePoint> routePoints)
        {
            return await Globals.RouteDatabase.SaveRoute(routePoints).ConfigureAwait(false);
        }

        private async Task<List<Route>> GetSavedRoutes()
        {
            return await Globals.RouteDatabase.GetDbRoutes().ConfigureAwait(false);
        }

        private async Task<List<Polyline>> GetLinesFromRoute(List<Route> routes)
        {
            return await Task.Run(() => routes.Select(CreatePolyline).ToList());
        }

        private void LoadCurrentRoute(List<RoutePoint> routePoints)
        {
            var line = CreatePolyline(new Route
            {
                Color = Globals.DefaultColor.ToHex(),
                RoutePointsBlob = CsExtensions.CompactRoutePointList(routePoints)
            });

            MyMap.MapElements.Add(line);
        }

        private async Task LoadRoutes(List<Route> routes)
        {
            var lines = await GetLinesFromRoute(routes);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                foreach (var polyline in lines)
                    MyMap.MapElements.Add(polyline);
            });
        }

        private void MoveToUserLocation()
        {
            try
            {
                var currentLocation = CsService.Instance.Location.CurrentLocation;

                if (currentLocation == null)
                    CrossToastPopUp.Current.ShowToastWarning("Requested Location is currently null");
                else
                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(currentLocation.Latitude,
                        currentLocation.Longitude), Distance.FromMiles(0.2)));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        private Task TogglePancakeView()
        {
            var pancakeHeight = MyPancakeView.Height;

            // var stackBottom = MyStackLayout.Bounds.Bottom;
            // var pancakeTx = MyPancakeView.TranslationX;
            // var pancakeTy = MyPancakeView.TranslationY;

            if (_vm.CanCollapse)
            {
                var newTranslationY = pancakeHeight - (ContainerStack.Height + 2) - MyPancakeView.Margin.Bottom;
                MyPancakeView.TranslateTo(MyPancakeView.TranslationX, newTranslationY, easing: Easing.SinOut);
                _vm.CanCollapse = false;
            }
            else
            {
                MyPancakeView.TranslateTo(0, 0, easing: Easing.SinIn);
                _vm.CanCollapse = true;
            }

            _toggleBusy = false;

            //Console.WriteLine($"{stackBottom} | {pancakeHeight} | {pancakeTx} | {pancakeTy}");
            return Task.CompletedTask;
        }

        private async Task QuitPage()
        {
            var ok = await DisplayAlert("Leaving Page",
                "Do you really want to quit?", "Yes", "Cancel");

            if (ok)
            {
                // Stop getting location updates
                CsService.Instance.Location.StopRequestingLocation();

                await Navigation.PopAsync();
            }
        }

        #endregion


        #region OnAppearing + Disappearing

        protected override void OnAppearing()
        {
            SubscribeToViewEventHandlers();

            // Capture screen WakeLock
            DeviceDisplay.KeepScreenOn = true;

            // Start getting location updates
            CsService.Instance.Location.StartRequestingLocation();

            // Try to move to current location
            MoveToUserLocation();

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            // Release screen WakeLock
            DeviceDisplay.KeepScreenOn = false;

            UnSubscribeFromViewEventHandlers();

            base.OnDisappearing();
        }

        private void SubscribeToViewEventHandlers()
        {
            if (_didLoadEventHandlers) return;

            _toolBarQuitItem.Clicked += QuitToolbarItem_Clicked;
            _autoAddWayPointsButton.Clicked += AutoAdd_OnClicked;
            _stopTimerButton.Clicked += StopTimer_OnClicked;
            _addMarkerButton.Clicked += AddMarker_OnClicked;
            _saveWayPointsButton.Clicked += SaveWayPoints_OnClicked;
            _clearMapButton.Clicked += ClearMap_OnClicked;
            _clearSavedWayPointsButton.Clicked += ClearSavedWayPoints_OnClicked;
            _clearCurrentWayPointsButton.Clicked += ClearCurrentWayPoints_OnClicked;
            _loadSavedMarkersButton.Clicked += LoadSavedMarkers_OnClicked;
            _reloadCurrentMarkersButton.Clicked += ReloadCurrentMarkers_OnClicked;
            _emailGraphButton.Clicked += EmailGraphButtonOnClicked;

            _didLoadEventHandlers = true;
        }

        private void UnSubscribeFromViewEventHandlers()
        {
            if (!_didLoadEventHandlers) return;

            _toolBarQuitItem.Clicked -= QuitToolbarItem_Clicked;
            _autoAddWayPointsButton.Clicked -= AutoAdd_OnClicked;
            _stopTimerButton.Clicked -= StopTimer_OnClicked;
            _addMarkerButton.Clicked -= AddMarker_OnClicked;
            _saveWayPointsButton.Clicked -= SaveWayPoints_OnClicked;
            _clearMapButton.Clicked -= ClearMap_OnClicked;
            _clearSavedWayPointsButton.Clicked -= ClearSavedWayPoints_OnClicked;
            _clearCurrentWayPointsButton.Clicked -= ClearCurrentWayPoints_OnClicked;
            _loadSavedMarkersButton.Clicked -= LoadSavedMarkers_OnClicked;
            _reloadCurrentMarkersButton.Clicked -= ReloadCurrentMarkers_OnClicked;
            _emailGraphButton.Clicked -= EmailGraphButtonOnClicked;

            _didLoadEventHandlers = false;
        }

        #endregion


        #region View Event Handlers

        private void AddMarker_OnClicked(object sender, EventArgs e)
        {
            AddRoutePoint();
        }

        private void AutoAdd_OnClicked(object sender, EventArgs e)
        {
            if (_timerStarted == false)
            {
                StartTimer();
                CrossToastPopUp.Current.ShowToastSuccess("Started Timer!");
            }

            _vm.AutoAddWayPoints = !_vm.AutoAddWayPoints;
        }

        private void StopTimer_OnClicked(object sender, EventArgs e)
        {
            if (_timerStarted)
            {
                StopTimer();
                CrossToastPopUp.Current.ShowToastSuccess("Stopped Timer!");
            }
            else
            {
                CrossToastPopUp.Current.ShowToastMessage("Timer Not Started");
            }
        }

        private void TogglePancakeView_OnTapped(object sender, EventArgs e)
        {
            _ = TogglePancakeView();
        }

        private async void SaveWayPoints_OnClicked(object sender, EventArgs e)
        {
            // Save current Markers to Db
            var routePoints = _vm.CurrentRoutePoints.ToList();
            if (routePoints == null || routePoints.Count < 2)
            {
                CrossToastPopUp.Current.ShowToastMessage("Too few RoutePoints to save yet; need at least 2");
                return;
            }

            var savedCount = await SaveRoute(routePoints);
            if (savedCount > 0)
            {
                MyMap.MapElements.Clear();
                _vm.CurrentRoutePoints.Clear();
                CrossToastPopUp.Current.ShowToastSuccess("Saved WayPoints!");
            }

            // Get saved Markers
            var saved = await GetSavedRoutes();
            await LoadRoutes(saved);
        }

        private void ClearMap_OnClicked(object sender, EventArgs e)
        {
            MyMap.MapElements.Clear();
            CrossToastPopUp.Current.ShowToastSuccess("Cleared Map!");
        }

        private async void ClearSavedWayPoints_OnClicked(object sender, EventArgs e)
        {
            var ok = await DisplayAlert("Warning", "Clear all saved WayPoints?", "Yes", "Cancel");

            if (ok)
            {
                await Globals.RouteDatabase.ClearDb();
                CrossToastPopUp.Current.ShowToastSuccess("Cleared Saved WayPoints!");
            }
        }

        private async void ClearCurrentWayPoints_OnClicked(object sender, EventArgs e)
        {
            var ok = await DisplayAlert("Warning", "Clear current WayPoints?", "Yes", "Cancel");

            if (ok)
            {
                _vm.CurrentRoutePoints.Clear();
                CrossToastPopUp.Current.ShowToastSuccess("Cleared Current WayPoints!");
            }
        }

        private async void LoadSavedMarkers_OnClicked(object sender, EventArgs e)
        {
            // Get saved markers from Db
            var saved = await GetSavedRoutes();
            if (saved == null || saved.Count < 1)
            {
                CrossToastPopUp.Current.ShowToastMessage("No Saved Markers to load yet!");
                return;
            }

            await LoadRoutes(saved);
            CrossToastPopUp.Current.ShowToastSuccess("Loaded Saved Markers!");
        }

        private void ReloadCurrentMarkers_OnClicked(object sender, EventArgs e)
        {
            MyMap.MapElements.Clear();
            LoadCurrentRoute(_vm.CurrentRoutePoints.ToList());
            CrossToastPopUp.Current.ShowToastSuccess("Reloaded Current Markers!");
        }

        private async void EmailGraphButtonOnClicked(object sender, EventArgs e)
        {
            var currentLocation = CsService.Instance.Location.CurrentLocation;

            var dbRoutes = await Globals.RouteDatabase.GetDbRoutes();
            if (dbRoutes == null || dbRoutes.Count < 1)
            {
                CrossToastPopUp.Current.ShowToastMessage("No Saved Routes Yet!");
                return;
            }

            if (currentLocation == null)
            {
                CrossToastPopUp.Current.ShowToastMessage("Current Location Null!");
                return;
            }

            var json = JsonConvert.SerializeObject(
                new ExportedRoutesJson(DateTime.Now, currentLocation.Latitude,
                    currentLocation.Longitude, dbRoutes), Formatting.Indented);

            var file = Path.Combine(FileSystem.CacheDirectory, Constants.ExportMarkersJsonFilename);

            using (var writer = new StreamWriter(file, false))
            {
                await writer.WriteAsync(json);
            }

            var message = new EmailMessage
            {
                Subject = "CS253ResearchProj",
                Body = "Exported Routes!",
                BodyFormat = EmailBodyFormat.PlainText
            };

            message.Attachments.Add(new EmailAttachment(file));

            await Email.ComposeAsync(message);
        }

        private async void QuitToolbarItem_Clicked(object sender, EventArgs e)
        {
            await QuitPage();
        }

        #endregion
    }
}