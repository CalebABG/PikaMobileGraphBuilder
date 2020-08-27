using CS253ShortestPath.Services;

namespace CS253ShortestPath.iOS
{
    public class CsServiceIos : CsService
    {
        private CsServiceIos()
        {
            if (_instance != null) return;

            _instance = this;
            _instance.Location = new LocationManagerIos();
        }

        public static void Init()
        {
            _ = new CsServiceIos();
        }
    }
}