using CS253ShortestPath.Models;

namespace CS253ShortestPath.Contracts
{
    public interface ILocation
    {
        RoutePoint? CurrentLocation { get; }
        void StartRequestingLocation();
        void StopRequestingLocation();
    }
}