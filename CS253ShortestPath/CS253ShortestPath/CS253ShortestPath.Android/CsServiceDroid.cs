using CS253ShortestPath.Services;

namespace CS253ShortestPath.Droid
{
    public class CsServiceDroid : CsService
    {
        private CsServiceDroid()
        {
            if (_instance != null) return;

            _instance = this;
            _instance.Location = new LocationManagerDroid();
        }

        public static void Init()
        {
            _ = new CsServiceDroid();
        }
    }
}