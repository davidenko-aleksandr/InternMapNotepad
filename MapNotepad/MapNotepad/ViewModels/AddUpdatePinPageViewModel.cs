using MapNotepad.Models;
using MapNotepad.Sevices.PinServices;
using MapNotepad.Sevices.Settings;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace MapNotepad.ViewModels
{
    public class AddUpdatePinPageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IPinService _pinService;

        private ObservableCollection<Pin> _myPins;
        private string _label = string.Empty;
        private string _description = string.Empty;
        private double _lalitude;
        private double _longitude;

        private ICommand _getPositionCommand;
        private ICommand _savePinCommand;

        public string Lable
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public double Latitude
        {
            get { return _lalitude; }
            set { SetProperty(ref _lalitude, value); }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }

        public ObservableCollection<Pin> MyPins
        {
            get { return _myPins; }
            set { SetProperty(ref _myPins, value); }
        }

        public AddUpdatePinPageViewModel(INavigationService navigationService,
                                         IPinService pinService)
        {
            _navigationService = navigationService;
            _pinService = pinService;
            _myPins = new ObservableCollection<Pin>();
        }

        public ICommand GetPositionCommand => _getPositionCommand ?? (_getPositionCommand = new Command<Position>(GetPosition));

        public ICommand SavePinCommand => _savePinCommand ?? (_savePinCommand = new Command(
                        async () => await SavePinAsync()));

        private async Task SavePinAsync()
        {
            await SavePinToDB();
            await _navigationService.NavigateAsync("MainTabbedPageView?selectedTab=MapPageView");
        }

        private void GetPosition(Position position)
        {                        
            Latitude = position.Latitude;
            Longitude = position.Longitude;

            Pin p = new Pin
            {
                Position = new Position(Latitude, Longitude),
                Label = Lable,
                Address = Description
            };

            MyPins.Clear();
            MyPins.Add(p);
        }

        private async Task SavePinToDB()
        {
            PinGoogleMap pin = new PinGoogleMap
            {
                Label = Lable,
                Description = Description,
                Latitude = Latitude,
                Longitude = Longitude
            };

            await _pinService.AddPinToDBAsync(pin);
        }
    }
}
