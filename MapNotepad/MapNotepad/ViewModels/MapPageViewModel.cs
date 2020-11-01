using MapNotepad.Models;
using Prism.Navigation;
using System.Collections.ObjectModel;
using Xamarin.Forms.GoogleMaps;

namespace MapNotepad.ViewModels
{
    public class MapPageViewModel : BaseViewModel
    {
        private PinGoogleMapModel _pinGoogleMap;

        private ObservableCollection<Pin> _myPins;
        public ObservableCollection<Pin> MyPins
        {
            get { return _myPins; }
            set { SetProperty(ref _myPins, value); }
        }

        public MapPageViewModel()
        {
            _myPins = new ObservableCollection<Pin>();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Pin pin = new Pin();
            _pinGoogleMap = (PinGoogleMapModel)parameters[Constants.SELECT_PIN];

            if (_pinGoogleMap != null)
            {
                pin.Position = new Position(_pinGoogleMap.Latitude, _pinGoogleMap.Longitude);
                pin.Label = _pinGoogleMap.Label;
                pin.Address = _pinGoogleMap.Description;
                pin.IsVisible = true;
                MyPins.Clear();
                MyPins.Add(pin);
            }
        }
    }
}
