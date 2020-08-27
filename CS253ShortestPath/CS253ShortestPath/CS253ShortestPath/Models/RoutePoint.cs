using System;

namespace CS253ShortestPath.Models
{
    public class RoutePoint
    {
        public RoutePoint()
        {
            // Default 22 for Length
            Id = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 16);
        }

        public string Id { get; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"Id={Id}, Lat={Latitude}, Lon={Longitude}";
        }
    }
}