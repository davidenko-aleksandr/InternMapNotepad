using MapNotepad.Models;
using MapNotepad.Sevices.PinServices;
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
        private readonly IPinService _pinService;

        private PinGoogleMapModel _pinGoogleMapModel;
        
        public AddUpdatePinPageViewModel(
                                         IPinService pinService,
                                         INavigationService navigationService) : base(navigationService)
        {
            _pinService = pinService;
            _myPins = new ObservableCollection<Pin>();
            _pinGoogleMapModel = new PinGoogleMapModel();
        }

        #region -- Public properties --
        private string _label = string.Empty;
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
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

        private ICommand _getPositionCommand;
        public ICommand GetPositionCommand => _getPositionCommand ??= new Command<Position>(OnGetPositionCommand);

        private ICommand _savePinCommand;
        public ICommand SavePinCommand => _savePinCommand ??= new Command(OnSavePinCommandAsync);

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new Command(OnComeBackCommandAsync);

        #endregion

        private async void OnSavePinCommandAsync()
        {
            await SavePinToDBAsync();
            await _navigationService.GoBackAsync();
        }

        private async void OnComeBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }
        
        private void OnGetPositionCommand(Position position)
        {                        
            Latitude = position.Latitude;
            Longitude = position.Longitude;

            Pin p = new Pin
            {
                Position = new Position(Latitude, Longitude),
                Label = Label,
                Address = Description
            };

            MyPins.Clear();
            MyPins.Add(p);
        }

        private async Task SavePinToDBAsync()
        {
            if (_pinGoogleMapModel != null)
            {
                _pinGoogleMapModel.Label = Label;
                _pinGoogleMapModel.Description = Description;
                _pinGoogleMapModel.Latitude = Latitude;
                _pinGoogleMapModel.Longitude = Longitude;

                await _pinService.AddOrUpdatePinInDBAsync(_pinGoogleMapModel);
            }
            else
            {
                PinGoogleMapModel pin = new PinGoogleMapModel
                {
                    Label = Label,
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
                Label = _pinGoogleMapModel.Label;
                Description = _pinGoogleMapModel.Description;
                Latitude = _pinGoogleMapModel.Latitude;
                Longitude = _pinGoogleMapModel.Longitude;
            }
        }
    }
}
