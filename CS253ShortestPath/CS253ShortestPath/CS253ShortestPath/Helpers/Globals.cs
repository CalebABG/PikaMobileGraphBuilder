using System;
using System.IO;
using CS253ShortestPath.Services;
using Xamarin.Forms;

namespace CS253ShortestPath.Helpers
{
    public static class Globals
    {
        private static RouteDatabase _routeDatabase = null!;
        public static RouteDatabase RouteDatabase => _routeDatabase ??= new RouteDatabase();

        public static Color DefaultColor { get; } = Color.Firebrick;

        public static string DatabasePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Constants.DatabaseFilename);
    }
}