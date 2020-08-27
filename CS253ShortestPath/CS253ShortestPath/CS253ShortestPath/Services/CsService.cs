using System;
using CS253ShortestPath.Contracts;

namespace CS253ShortestPath.Services
{
    public class CsService
    {
        protected static CsService _instance;
        public ILocation Location;

        protected CsService()
        {
        }

        public static CsService Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("Instance is null!");

                return _instance;
            }
        }
    }
}