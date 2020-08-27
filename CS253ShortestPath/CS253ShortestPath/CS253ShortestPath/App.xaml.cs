using System;
using AsyncAwaitBestPractices;
using CS253ShortestPath.Pages;
using Xamarin.Forms;

namespace CS253ShortestPath
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex =>
            {
                if (ex == null) return;
                Console.Error.WriteLine("SafeFireAndForget: Exception - " +
                                        $"Message={ex.Message} -- StackTrace={ex.StackTrace}");
            });

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            Console.WriteLine("CS253ShortestPath:OnStart - App Started");
        }

        protected override void OnSleep()
        {
            Console.WriteLine("CS253ShortestPath:OnSleep - App Entered Sleep");
        }

        protected override void OnResume()
        {
            Console.WriteLine("CS253ShortestPath:OnResume - App Resumed");
        }
    }
}