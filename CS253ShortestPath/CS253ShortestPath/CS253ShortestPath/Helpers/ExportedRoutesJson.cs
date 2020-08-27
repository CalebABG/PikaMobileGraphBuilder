using System;
using System.Collections.Generic;
using CS253ShortestPath.Models;
using Newtonsoft.Json;

namespace CS253ShortestPath.Helpers
{
    public class ExportedRoutesJson
    {
        public ExportedRoutesJson(DateTime exportDate, double mapCenterLat, double mapCenterLong,
            List<Route> routes)
        {
            ExportDate = exportDate;
            MapCenterLat = mapCenterLat;
            MapCenterLong = mapCenterLong;
            Routes = routes ?? throw new ArgumentNullException(nameof(routes));
        }

        [JsonProperty("export_date")] public DateTime ExportDate { get; set; }

        [JsonProperty("map_center_lat")] public double MapCenterLat { get; set; }

        [JsonProperty("map_center_long")] public double MapCenterLong { get; set; }

        [JsonProperty("routes")] public List<Route> Routes { get; set; }
    }
}