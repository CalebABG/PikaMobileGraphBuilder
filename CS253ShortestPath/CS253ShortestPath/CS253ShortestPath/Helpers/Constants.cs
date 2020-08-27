using SQLite;

namespace CS253ShortestPath.Helpers
{
    public static class Constants
    {
        public const string DatabaseFilename = "CS253ProjRoutes.db3";

        // If you change this filename, change must also be reflected in webMarkerVis.py so it can find the json file
        public const string ExportMarkersJsonFilename = "exported-routes.json";

        public const float RouteStrokeWidth = 15;

        public const SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLiteOpenFlags.SharedCache;
    }
}