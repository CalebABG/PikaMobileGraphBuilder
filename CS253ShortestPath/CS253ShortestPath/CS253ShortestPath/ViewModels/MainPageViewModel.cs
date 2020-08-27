using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using CS253ShortestPath.Helpers;
using CS253ShortestPath.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CS253ShortestPath.ViewModels
{
    public class MainPageViewModel : CsViewModelBase
    {
        public MainPageViewModel()
        {
            CheckPermissionsCommand = new AsyncCommand(CheckLocationPermissions, obj => IsNotBusy);
        }

        public AsyncCommand CheckPermissionsCommand { get; }
        public TimeSpan MainPageImageCacheDuration { get; set; } = TimeSpan.FromHours(5);

        private async Task CheckLocationPermissions()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                // Check permissions before
                var locationPermissions = await CsExtensions.CheckPermissions(new Permissions.LocationAlways());
                switch (locationPermissions)
                {
                    case PermissionStatus.Granted:
                        await Application.Current.MainPage.Navigation.PushAsync(new MapToGraph());
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    $"{nameof(MainPageViewModel)}:{nameof(CheckLocationPermissions)} - Exception -- {e.Message}, Stack: {e.StackTrace}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}