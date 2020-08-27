using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using CS253ShortestPath.CustomRenderers;
using CS253ShortestPath.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ResearchMap), typeof(ResearchMapRendererDroid))]

namespace CS253ShortestPath.Droid
{
    public class ResearchMapRendererDroid : MapRenderer, GoogleMap.IOnMarkerClickListener
    {
        public ResearchMapRendererDroid(Context context) : base(context)
        {
        }

        public bool OnMarkerClick(Marker marker)
        {
            marker.ShowInfoWindow();
            return marker.IsInfoWindowShown;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                if (NativeMap != null)
                    NativeMap.MyLocationChange -= NativeMapOnMyLocationChange;

            if (e.NewElement != null)
            {
                var formsMap = (ResearchMap) e.NewElement;
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.MyLocationChange += NativeMapOnMyLocationChange;
            NativeMap.SetOnMarkerClickListener(this);
        }

        private void NativeMapOnMyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            if (e?.Location == null) return;

            if (Element is ResearchMap researchMap)
                if (researchMap.CameraFollowsUser)
                {
                    var newLat = e.Location.Latitude;
                    var newLong = e.Location.Longitude;
                    var bearing = e.Location.Bearing;

                    var cameraPositionBuilder = CameraPosition.InvokeBuilder();
                    cameraPositionBuilder.Target(new LatLng(newLat, newLong));
                    cameraPositionBuilder.Bearing(bearing);
                    cameraPositionBuilder.Zoom(researchMap.CameraZoom);

                    var cameraPosition = cameraPositionBuilder.Build();
                    var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                    NativeMap.AnimateCamera(cameraUpdate);
                }

            // Console.WriteLine($"ResearchMapRenderer: Lat={location.Latitude}, Long={location.Longitude}, Alt={location.Altitude}, Spd={location.Speed}, Acc={location.Accuracy}, Hdg={location.Bearing}");
        }
    }
}