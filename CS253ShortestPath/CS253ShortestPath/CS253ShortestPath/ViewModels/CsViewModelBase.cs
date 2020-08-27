using GalaSoft.MvvmLight;

namespace CS253ShortestPath.ViewModels
{
    public class CsViewModelBase : ViewModelBase
    {
        private bool _isBusy;

        private string _title;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (Set(ref _isBusy, value)) RaisePropertyChanged(nameof(IsNotBusy));
            }
        }

        public bool IsNotBusy => !IsBusy;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public virtual void OnViewModelAppearing()
        {
        }

        public virtual void OnViewModelDisappearing()
        {
        }
    }
}