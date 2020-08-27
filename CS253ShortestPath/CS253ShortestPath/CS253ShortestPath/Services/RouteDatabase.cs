using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CS253ShortestPath.Helpers;
using CS253ShortestPath.Models;
using Xamarin.Forms;

namespace CS253ShortestPath.Services
{
    public class RouteDatabase : LocalDatabase<Route>
    {
        private static readonly Random CrRandom = new Random();

        private static Color RandColor =>
            Color.FromRgb((byte) CrRandom.Next(), (byte) CrRandom.Next(), (byte) CrRandom.Next());


        public Task<List<Route>> GetDbRoutes()
        {
            return Database.Table<Route>().ToListAsync();
        }

        public Task<int> SaveRoute(List<RoutePoint> wayPoints)
        {
            if (wayPoints == null || wayPoints.Count < 1) return Task.FromResult(0);

            var route = new Route
            {
                Color = RandColor.ToHex(),
                RoutePointsBlob = CsExtensions.CompactRoutePointList(wayPoints)
            };

            return Database.InsertAsync(route);
        }

        public Task<int> ClearDb()
        {
            return Database.DeleteAllAsync<Route>();
        }
    }
}