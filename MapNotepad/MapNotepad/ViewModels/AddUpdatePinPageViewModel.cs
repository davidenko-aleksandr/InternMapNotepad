using MapNotepad.Models;
using MapNotepad.Sevices.PinServices;
using MapNotepad.Views;
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

        private PinGoogleMapModel _pinGoogleMapModel;

        private ICommand _getPositionCommand;
        private ICommand _savePinCommand;

        private string _lable = string.Empty;
        public string Lable
        {
            get { return _lable; }
            set { SetProperty(ref _lable, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private double _lalitude;
        public double Latitude
        {
            get { return _lalitude; }
            set { SetProperty(ref _lalitude, value); }
        }

        private double _longitude;
        public double Longitude
        {
            get { return _longitude; }
            set { SetProperty(ref _longitude, value); }
        }

        private ObservableCollection<Pin> _myPins;

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
            _pinGoogleMapModel = new PinGoogleMapModel();
        }

        public ICommand GetPositionCommand => _getPositionCommand ?? (_getPositionCommand = new Command<Position>(GetPosition));

        public ICommand SavePinCommand => _savePinCommand ?? (_savePinCommand = new Command(
                                          async () => await SavePinAsync()));

        private async Task SavePinAsync()
        {
            await SavePinToDB();
            await _navigationService.NavigateAsync($"{nameof(MainTabbedPageView)}?selectedTab={nameof(PinPageView)}");
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
            if (_pinGoogleMapModel != null)
            {
                _pinGoogleMapModel.Label = Lable;
                _pinGoogleMapModel.Description = Description;
                _pinGoogleMapModel.Latitude = Latitude;
                _pinGoogleMapModel.Longitude = Longitude;

                await _pinService.AddOrUpdatePinInDBAsync(_pinGoogleMapModel);
            }
            else
            {
                PinGoogleMapModel pin = new PinGoogleMapModel
                {
                    Label = Lable,
                    Description = Description,
                    Latitude = Latitude,
                    Longitude = Longitude
                };

                await _pinService.AddOrUpdatePinInDBAsync(pin);
            }            
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.SELECT_PIN, out _pinGoogleMapModel) && _pinGoogleMapModel != null)
            {
                Lable = _pinGoogleMapModel.Label;
                Description = _pinGoogleMapModel.Description;
                Latitude = _pinGoogleMapModel.Latitude;
                Longitude = _pinGoogleMapModel.Longitude;
            }
        }
    }
}
