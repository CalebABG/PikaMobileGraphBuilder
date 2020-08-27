using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CS253ShortestPath.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CS253ShortestPath.Helpers
{
    public static class CsExtensions
    {
        #region Permissions

        public static async Task<PermissionStatus> CheckPermissions<TPermission>(TPermission permission)
            where TPermission : Permissions.BasePermission
        {
            var permissionStatus = await permission.CheckStatusAsync();

            var request = false;

            var title = $"{permission} Permission";
            var question = $"Please go into Settings and turn on {permission} for the app";
            var positive = "Settings";
            var negative = "Maybe Later";

            if (permissionStatus == PermissionStatus.Denied)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                    if (task == null)
                        return permissionStatus;

                    var result = await task;
                    if (result) AppInfo.ShowSettingsUI();

                    return permissionStatus;
                }

                request = true;
            }

            if (request || permissionStatus != PermissionStatus.Granted)
            {
                var newStatus = await permission.RequestAsync();

                permissionStatus = newStatus;

                if (permissionStatus != PermissionStatus.Granted)
                {
                    permissionStatus = newStatus;

                    var displayAlertTask =
                        Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                    
                    if (displayAlertTask == null)
                        return permissionStatus;

                    var result = await displayAlertTask;
                    if (result) AppInfo.ShowSettingsUI();

                    return permissionStatus;
                }
            }

            return permissionStatus;
        }

        #endregion

        #region Base64 Encode_Decode

        public static string EncodeBase64(this string text)
        {
            return ToBase64(text, Encoding.UTF8);
        }

        public static string DecodeBase64(this string text)
        {
            return TryParseBase64(text, Encoding.UTF8);
        }

        private static string ToBase64(this string text, Encoding encoding)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var textAsBytes = encoding.GetBytes(text);
            return Convert.ToBase64String(textAsBytes);
        }

        private static string TryParseBase64(this string text, Encoding encoding)
        {
            string decodedText = null!;

            if (string.IsNullOrEmpty(text)) decodedText = text;

            try
            {
                var textAsBytes = Convert.FromBase64String(text);
                decodedText = encoding.GetString(textAsBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(CsExtensions)}:{nameof(TryParseBase64)} - Exception -- {ex}");
            }

            return decodedText;
        }

        public static string CompactRoutePointList(List<RoutePoint> routePoints)
        {
            return JsonConvert.SerializeObject(routePoints).EncodeBase64();
        }

        public static List<RoutePoint> ExpandRoutePointList(string encodedRoutePoints)
        {
            return JsonConvert.DeserializeObject<List<RoutePoint>>(encodedRoutePoints.DecodeBase64());
        }

        #endregion
    }
}