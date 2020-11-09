using MapNotepad.Extentions;
using MapNotepad.Models;
using MapNotepad.Sevices.MapPositionService;
using MapNotepad.Sevices.PermissionServices;
using MapNotepad.Sevices.PinServices;
using MapNotepad.Views.PopupPageViews;
using Prism.Navigation;
using System;
using System.Collections.Generic;
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
        private readonly IPermissionService _permissionService;

        private PinGoogleMapModel _pinGoogleMapModel;
        private Pin _pin;

        public MapPageViewModel(
                                IPinService pinService, 
                                IMapPositionService mapPositionService,
                                IPermissionService permissionService,
                                INavigationService navigationService) : base(navigationService)
        {
            _pinService = pinService;
            _myPins = new ObservableCollection<Pin>();
            _pinGoogleMapModel = new PinGoogleMapModel();
            _mapPositionService = mapPositionService;
            _permissionService = permissionService;
            _pin = new Pin();
        }

        #region -- Public properties --

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

        private string _selectedPinLabel = string.Empty;
        public string SelectedPinLabel
        {
            get { return _selectedPinLabel; }
            set { SetProperty(ref _selectedPinLabel, value); }
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

        private bool _isMyLocationEnabled;
        public bool IsMyLocationEnabled
        {
            get { return _isMyLocationEnabled; }
            set { SetProperty(ref _isMyLocationEnabled, value); }
        }

        private ICommand _selectedPinCommand;
        public ICommand SelectedPinCommand => _selectedPinCommand ??= new Command<Pin>(OnSelectedPinCommand);

        private ICommand _searchPinCommand;
        public ICommand SearchPinCommand => _searchPinCommand ??= new Command(OnSearchPinCommandAsync);

        private ICommand _mapClickedCommand;
        public ICommand MapClickedCommand => _mapClickedCommand ??= new Command(OnCloseFrameCommand);

        private ICommand _getPositionComaand;
        public ICommand GetPositionComaand => _getPositionComaand ??= new Command<CameraPosition>(OnGetPositionCommand);

        private ICommand _openAddNoteViewPageCommand;
        public ICommand OpenAddNoteViewPageCommand => _openAddNoteViewPageCommand ??= new Command(OnOpenAddNoteViewPageCommandAsync);

        #endregion

        private async void OnOpenAddNoteViewPageCommandAsync()
        {
            int pinId = await _pinService.GetPinId(_pin.Position.Latitude, _pin.Position.Longitude);

            NavigationParameters parameters = new NavigationParameters { { Constants.SELECTED_PIN, pinId } };

            await _navigationService.NavigateAsync($"{nameof(AddNotePageView)}", parameters);
        }

        private void OnGetPositionCommand(CameraPosition position)
        { 
            _mapPositionService.ReadCameraPosition(position.Target.Latitude, position.Target.Longitude, position.Zoom);            
        }

        private void OnCloseFrameCommand()
        {
            IsVisibleFrame = false;
        }

        private void OnSelectedPinCommand(Pin selectedPin)
        {
            _pin = selectedPin;
            SelectedPinLabel = _pin.Label;
            SelectedPinDescription = _pin.Address;
            SelectedPinLatitude = _pin.Position.Latitude.ToString();
            SelectedPinLongitude = _pin.Position.Longitude.ToString();

            IsVisibleFrame = true;
        }

        private async void OnSearchPinCommandAsync()
        {
            await InitCollectionPinAsync();       
        }

        public async Task InitCollectionPinAsync()
        {
            IEnumerable<PinGoogleMapModel> collection = String.IsNullOrEmpty(SearchFilter)
                ? await _pinService.GetPinsFromDBAsync()
                : await _pinService.GetPinsFromDBAsync(SearchFilter);

            ObservableCollection<PinGoogleMapModel> pinGoogleMapModels = new ObservableCollection<PinGoogleMapModel>(collection);

            MyPins.Clear();

            foreach (var item in pinGoogleMapModels)
            {                
                MyPins.Add(PinGoogleMapModelExtension.ToPin(item));
            }           
        }

        #region -- OnNavigationTO/From --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            IsMyLocationEnabled = await _permissionService.PermissionRequestAsync();

            if (parameters.TryGetValue(Constants.SELECTED_PIN, out _pinGoogleMapModel) && _pinGoogleMapModel != null)
            { 
                Pin pin = new Pin
                {
                    Position = new Position(_pinGoogleMapModel.Latitude, _pinGoogleMapModel.Longitude),
                    Label = _pinGoogleMapModel.Label,
                    Address = _pinGoogleMapModel.Description
                };
                
                await InitCollectionPinAsync();

                MyPins.Add(pin);

                CameraPosition = new Position(_pinGoogleMapModel.Latitude, _pinGoogleMapModel.Longitude);
                CameraUpdate = new CameraPosition(CameraPosition, 15.0);                               
            }
            else
            {   
                await InitCollectionPinAsync();

                Position lastPosition = new Position(_mapPositionService.Latitude, _mapPositionService.Longitude);

                CameraUpdate = new CameraPosition(lastPosition, _mapPositionService.Zoom);
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            IsVisibleFrame = false;
        }
        #endregion
    }
}
