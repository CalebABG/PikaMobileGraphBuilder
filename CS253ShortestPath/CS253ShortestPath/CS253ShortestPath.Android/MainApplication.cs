using System;
using Android.App;
using Android.Runtime;
using Android.Util;
using Plugin.CurrentActivity;

namespace CS253ShortestPath.Droid
{
    #if DEBUG
    [Application(Debuggable = true)]
    #else
[Application(Debuggable = false)]
    #endif
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            CrossCurrentActivity.Current.Init(this);
            CrossCurrentActivity.Current.ActivityStateChanged += Current_ActivityStateChanged;
        }

        private void Current_ActivityStateChanged(object sender, ActivityEventArgs e)
        {
            Log.Debug("X:ShortestPath", $"Activity Changed: {e.Activity.LocalClassName} -  {e.Event}");
        }
    }
}