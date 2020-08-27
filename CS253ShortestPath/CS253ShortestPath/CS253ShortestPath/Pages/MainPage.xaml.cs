using CS253ShortestPath.ViewModels;
using Xamarin.Forms;

namespace CS253ShortestPath.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainPageViewModel();
        }
    }
}