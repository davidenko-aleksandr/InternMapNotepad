using MapNotepad.Models;
using MapNotepad.Resources;
using MapNotepad.Sevices.MapPositionService;
using MapNotepad.Sevices.PinServices;
using MapNotepad.Validators;
using Prism.Navigation;
using Prism.Services;
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
        private readonly IMapPositionService _mapPositionService;
        private readonly IPageDialogService _dialogService;

        private PinGoogleMapModel _pinGoogleMapModel;
        
        public AddUpdatePinPageViewModel(
                                         IPinService pinService,
                                         IPageDialogService dialogService,
                                         INavigationService navigationService,
                                         IMapPositionService mapPositionService) : base(navigationService)
        {
            _dialogService = dialogService;
            _pinService = pinService;
            _myPins = new ObservableCollection<Pin>();
            _pinGoogleMapModel = new PinGoogleMapModel();
            _mapPositionService = mapPositionService;
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

        private CameraPosition _cameraUpdate;
        public CameraPosition CameraUpdate
        {
            get { return _cameraUpdate; }
            set { SetProperty(ref _cameraUpdate, value); }
        }

        private ICommand _getPositionCommand;
        public ICommand GetPositionCommand => _getPositionCommand ??= new Command<Position>(OnGetPositionCommand);

        private ICommand _savePinCommand;
        public ICommand SavePinCommand => _savePinCommand ??= new Command(OnSavePinCommandAsync);

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new Command(OnComeBackCommandAsync);

        #endregion

        #region -- Private helpers --

        private async void OnSavePinCommandAsync()
        {
            bool isDataCorrect = await SavePinToDBAsync();

            if (isDataCorrect)
            {
                await _navigationService.GoBackAsync();
            }
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

        private async Task<bool> SavePinToDBAsync()
        {
            bool isDataCorrect;

            if (_pinGoogleMapModel != null)
            {
                _pinGoogleMapModel.Label = Label;
                _pinGoogleMapModel.Description = Description;
                _pinGoogleMapModel.Latitude = Latitude;
                _pinGoogleMapModel.Longitude = Longitude;

                isDataCorrect = await SaveOrShowAlert(_pinGoogleMapModel);
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

                isDataCorrect = await SaveOrShowAlert(pin);              
            }

            return isDataCorrect;
        }

        private async Task<bool> SaveOrShowAlert(PinGoogleMapModel pin)
        {
            bool isDataCorrect;

            if (PinDataValidator.ValidateLatitude(Latitude) && PinDataValidator.ValidateLongitude(Longitude))
            {
                isDataCorrect = true;
            }
            else
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertError, AppResources.AlertIncorrectPinData, AppResources.AlertOk);
                isDataCorrect = false;
            }
            if (isDataCorrect)
            {
                if (!string.IsNullOrEmpty(Label))
                {
                    await _pinService.AddOrUpdatePinInDBAsync(pin);
                }
                else
                {
                    await _dialogService.DisplayAlertAsync(AppResources.AlertError, AppResources.AlertIncorrectPinLabel, AppResources.AlertOk);
                    isDataCorrect = false;
                }
            }

            return isDataCorrect;
        }

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.SELECTED_PIN, out _pinGoogleMapModel) && _pinGoogleMapModel != null)
            {
                Label = _pinGoogleMapModel.Label;
                Description = _pinGoogleMapModel.Description;
                Latitude = _pinGoogleMapModel.Latitude;
                Longitude = _pinGoogleMapModel.Longitude;

                Pin p = new Pin
                {
                    Position = new Position(Latitude, Longitude),
                    Label = Label,
                    Address = Description
                };

                MyPins.Clear();
                MyPins.Add(p);
            }

            Position lastPosition = new Position(_mapPositionService.Latitude, _mapPositionService.Longitude);

            CameraUpdate = new CameraPosition(lastPosition, _mapPositionService.Zoom);
        }

        #endregion
    }
}
