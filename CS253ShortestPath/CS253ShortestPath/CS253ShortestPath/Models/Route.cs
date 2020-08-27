using System.Collections.Generic;
using CS253ShortestPath.Helpers;
using Newtonsoft.Json;
using SQLite;

namespace CS253ShortestPath.Models
{
    public class Route
    {
        [PrimaryKey] [AutoIncrement] public int Id { get; set; }

        public string Color { get; set; } = string.Empty;
        public string RoutePointsBlob { get; set; } /* Serialized/Deserialized List<RoutePoint> */

        [JsonIgnore] public List<RoutePoint> RoutePoints => CsExtensions.ExpandRoutePointList(RoutePointsBlob);
    }
}