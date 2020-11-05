using MapNotepad.Extentions;
using MapNotepad.Models;
using MapNotepad.Sevices.MapPositionService;
using MapNotepad.Sevices.PinServices;
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
        private readonly IMapPositionService _mapPositionService; 

        private PinGoogleMapModel _pinGoogleMapModel;

        #region -- Fields and Property --
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
        #endregion
        public MapPageViewModel(IPinService pinService, 
                                IMapPositionService mapPositionService)
        {
            _pinService = pinService;
            _myPins = new ObservableCollection<Pin>();
            _pinGoogleMapModel = new PinGoogleMapModel();
            _mapPositionService = mapPositionService;
        }

        private ICommand _selectedPinCommand;
        public ICommand SelectedPinCommand => _selectedPinCommand ??= new Command((Object obj) =>  SelectedPin(obj));

        private ICommand _searchPinCommand;
        public ICommand SearchPinCommand => _searchPinCommand ??= new Command(async () => await SearchPin());

        private ICommand _mapClickedCommand;
        public ICommand MapClickedCommand => _mapClickedCommand ??= new Command(CloseFrame);

        private ICommand _getPositionComaand;
        public ICommand GetPositionComaand => _getPositionComaand ??= new Command((Object obj) => GetPosition(obj));

        private void GetPosition(object obj)
        {
            if (obj is CameraPosition position)
            {
                _mapPositionService.ReadCameraPosition(position.Target.Latitude, position.Target.Longitude, position.Zoom);
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

        #region -- OnNavigationTO/From --

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

                Position lastPosition = new Position(_mapPositionService.Latitude(), _mapPositionService.Longitude());

                CameraUpdate = new CameraPosition(lastPosition, _mapPositionService.Zoom());

            }
        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            IsVisibleFrame = false;
        }
        #endregion
    }
}
