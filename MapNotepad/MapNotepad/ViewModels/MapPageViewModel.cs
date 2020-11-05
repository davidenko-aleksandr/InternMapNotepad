using MapNotepad.Extentions;
using MapNotepad.Models;
using MapNotepad.Sevices.PinServices;
using MapNotepad.Sevices.Settings;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace MapNotepad.ViewModels
{
    public class MapPageViewModel : BaseViewModel
    {
        private readonly IPinService _pinService;
        private readonly ISettingsService _settingsService;

        private PinGoogleMapModel _pinGoogleMapModel;

        private ICommand _selectedPinCommand;
        private ICommand _mapClickedCommand;
        private ICommand _searchPinCommand;

        private double _latitude;
        private double _longitude;
        private double _zoom;

        private ObservableCollection<Pin> _myPins;
        public ObservableCollection<Pin> MyPins
        {
            get { return _myPins; }
            set { SetProperty(ref _myPins, value); }
        }

        private Position _cameraPosition;
        public Position CameraPosition
        {
            get { return _cameraPosition; }
            set { SetProperty(ref _cameraPosition, value); }            
        }

        private CameraPosition _cameraUpdate;
        public CameraPosition CameraUpdate
        {
            get { return _cameraUpdate; }
            set { SetProperty(ref _cameraUpdate, value); }
        }

        private string _selectedPinLable = string.Empty;
        public string SelectedPinLable
        {
            get { return _selectedPinLable; }
            set { SetProperty(ref _selectedPinLable, value); }
        }

        private string _selectedPinDescription = string.Empty;
        public string SelectedPinDescription
        {
            get { return _selectedPinDescription; }
            set { SetProperty(ref _selectedPinDescription, value); }
        }

        private string _selectedPinLatitude = string.Empty;
        public string SelectedPinLatitude
        {
            get { return _selectedPinLatitude; }
            set { SetProperty(ref _selectedPinLatitude, value); }
        }

        private string _selectedPinLongitude = string.Empty;
        public string SelectedPinLongitude
        {
            get { return _selectedPinLongitude; }
            set { SetProperty(ref _selectedPinLongitude, value); }
        }

        private bool _isVisibleFrame = false;
        public bool IsVisibleFrame
        {
            get { return _isVisibleFrame; }
            set { SetProperty(ref _isVisibleFrame, value); }
        }

        private string _searchFilter = string.Empty;
        public string SearchFilter
        {
            get { return _searchFilter; }
            set { SetProperty(ref _searchFilter, value); }
        }
        public MapPageViewModel(IPinService pinService, ISettingsService settingsService)
        {
            _pinService = pinService;
            _myPins = new ObservableCollection<Pin>();
            _pinGoogleMapModel = new PinGoogleMapModel();
            _settingsService = settingsService;
        }

        public ICommand SelectedPinCommand => _selectedPinCommand ??= new Command((Object obj) =>  SelectedPin(obj));

        public ICommand SearchPinCommand => _searchPinCommand ??= new Command(async () => await SearchPin());

        public ICommand MapClickedCommand => _mapClickedCommand ??= new Command(CloseFrame);

        private ICommand _getPositionComaand;
        public ICommand GetPositionComaand => _getPositionComaand ??= new Command((Object obj) => GetPosition(obj));

        private void GetPosition(object obj)
        {
            if (obj is CameraPosition position)
            {
                _latitude = position.Target.Latitude;
                _longitude = position.Target.Longitude;
                _zoom = position.Zoom;
            }
        }

        private void CloseFrame()
        {
            IsVisibleFrame = false;
        }

        private void SelectedPin(object obj)
        {
            if (obj is Pin selectedPin)
            {
                SelectedPinLable = selectedPin.Label;
                SelectedPinDescription = selectedPin.Address;
                SelectedPinLatitude = selectedPin.Position.Latitude.ToString();
                SelectedPinLongitude = selectedPin.Position.Longitude.ToString();

                IsVisibleFrame = true;
            }
        }

        private async Task SearchPin()
        {
                MyPins.Clear();
                await InitCollectionPinAsync();       
        }

        public async Task InitCollectionPinAsync()
        {
            System.Collections.Generic.IEnumerable<PinGoogleMapModel> collection = String.IsNullOrEmpty(SearchFilter)
                ? await _pinService.GetPinsFromDBAsync()
                : await _pinService.GetPinsFromDBAsync(SearchFilter);
            ObservableCollection<PinGoogleMapModel> pinGoogleMapModels = new ObservableCollection<PinGoogleMapModel>(collection);

            foreach (var item in pinGoogleMapModels)
            {                
                MyPins.Add(PinGoogleMapModelExtention.ToPin(item));
            }           
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.SELECT_PIN, out _pinGoogleMapModel) && _pinGoogleMapModel != null)
            { 
                Pin pin = new Pin
                {
                    Position = new Position(_pinGoogleMapModel.Latitude, _pinGoogleMapModel.Longitude),
                    Label = _pinGoogleMapModel.Label,
                    Address = _pinGoogleMapModel.Description
                };
                MyPins.Clear();
                MyPins.Add(pin);

                CameraPosition = new Position(_pinGoogleMapModel.Latitude, _pinGoogleMapModel.Longitude);

                await InitCollectionPinAsync();                
            }
            else
            {
                MyPins.Clear();

                await InitCollectionPinAsync();

                Position lastPosition = new Position(_settingsService.MapLatitude, _settingsService.MapLongitude);

                CameraUpdate = new CameraPosition(lastPosition, _settingsService.MapZoom);

            }
        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            IsVisibleFrame = false;
            _settingsService.MapLatitude = _latitude;
            _settingsService.MapLongitude = _longitude;
            _settingsService.MapZoom = _zoom;
        }
    }
}
