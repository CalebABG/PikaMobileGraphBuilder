using System;
using CS253ShortestPath.CustomRenderers;
using CS253ShortestPath.iOS;
using MapKit;
using ObjCRuntime;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ResearchMap), typeof(ResearchMapRendererIos))]

namespace CS253ShortestPath.iOS
{
    public class ResearchMapRendererIos : MapRenderer
    {
        private static readonly string _annotationViewIdentifier = "wayPointViewIdentifier";
        private MKMapView _mapView;
        private ResearchMap _myMap;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                if (Control is MKMapView oldMapView)
                {
                    oldMapView.RemoveAnnotations(oldMapView.Annotations);
                    oldMapView.GetViewForAnnotation = null;

                    oldMapView.MapLoaded -= MapView_MapLoaded;
                    oldMapView.DidUpdateUserLocation -= MapViewOnDidUpdateUserLocation;
                }

            if (e.NewElement != null)
            {
                _mapView = Control as MKMapView;
                _myMap = e.NewElement as ResearchMap;

                if (_myMap != null)
                {
                }

                if (_mapView != null)
                {
                    _mapView.ShowsScale = true;
                    _mapView.UserTrackingMode = MKUserTrackingMode.FollowWithHeading;
                    _mapView.GetViewForAnnotation = GetViewForAnnotation;
                    _mapView.DidUpdateUserLocation += MapViewOnDidUpdateUserLocation;
                    _mapView.MapLoaded += MapView_MapLoaded;
                }
            }
        }

        private void MapView_MapLoaded(object sender, EventArgs e)
        {
        }

        private void MapViewOnDidUpdateUserLocation(object sender, MKUserLocationEventArgs e)
        {
            if (e.UserLocation == null || _myMap == null || _mapView == null) return;

            var trackingMode = _myMap.CameraFollowsUser
                ? MKUserTrackingMode.FollowWithHeading
                : MKUserTrackingMode.None;

            if (_mapView.UserTrackingMode != trackingMode)
                _mapView.SetUserTrackingMode(trackingMode, true);

            Console.WriteLine($"ResearchMapRendererIos:DidUpdateUserLocation: {e.UserLocation.Coordinate}");
        }

        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (AnnotationIsUserLocation(mapView, annotation))
                return null;

            var annotationView =
                mapView.DequeueReusableAnnotation(_annotationViewIdentifier) as MKPinAnnotationView;

            if (annotationView == null)
                annotationView = new MKPinAnnotationView(annotation, _annotationViewIdentifier);
            else
                annotationView.Annotation = annotation;

            if (annotation is WayPointMkAnnotation wayPointMkAnnotation)
            {
                annotationView.PinTintColor = wayPointMkAnnotation.PinColor;
                annotationView.CanShowCallout = true;
            }

            return annotationView;
        }

        private bool AnnotationIsUserLocation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (Runtime.GetNSObject(annotation.Handle) is MKUserLocation userLocationAnnotation)
                return userLocationAnnotation == mapView.UserLocation;

            return false;
        }
    }
}