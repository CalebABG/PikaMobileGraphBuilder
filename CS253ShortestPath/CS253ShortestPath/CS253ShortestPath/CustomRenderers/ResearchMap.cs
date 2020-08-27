using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CS253ShortestPath.CustomRenderers
{
    public class ResearchMap : Map
    {
        private static readonly MapSpan DefaultMapSpan
            = MapSpan.FromCenterAndRadius(new Position(41.762705, -72.671380), Distance.FromMiles(0.2));


        private ResearchMap(MapSpan region) : base(region)
        {
        }

        public ResearchMap() : this(DefaultMapSpan)
        {
        }

        #region BindableProps

        public static BindableProperty CameraFollowsUserProperty
            = BindableProperty.Create(
                nameof(CameraFollowsUser),
                typeof(bool),
                typeof(ResearchMap),
                true,
                BindingMode.TwoWay
            );

        public bool CameraFollowsUser
        {
            get => (bool) GetValue(CameraFollowsUserProperty);
            set => SetValue(CameraFollowsUserProperty, value);
        }

        public static BindableProperty CameraZoomProperty
            = BindableProperty.Create(
                nameof(CameraZoom),
                typeof(int),
                typeof(ResearchMap),
                16,
                BindingMode.TwoWay
            );

        public int CameraZoom
        {
            get => (int) GetValue(CameraZoomProperty);
            set => SetValue(CameraZoomProperty, value);
        }

        #endregion
    }
}