using MapNotepad.Models;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MapNotepad.Sevices.PinServices;
using MapNotepad.Views;
using Prism.Navigation.TabbedPages;
using MapNotepad.Services.AuthenticationServices;
using Acr.UserDialogs;
using MapNotepad.Resources;

namespace MapNotepad.ViewModels
{
    public class PinPageViewModel : BaseViewModel
    {
        private readonly IPinService _pinService;
        private readonly IUserAuthorization _userAuthorization;
        
        public PinPageViewModel(INavigationService navigationService,
                        IPinService pinService,
                        IUserAuthorization userAuthorization) : base(navigationService)
        {
            _pinService = pinService;
            _userAuthorization = userAuthorization;
        }

        #region -- Public properties --

        private string _searchFilter = string.Empty;
        public string SearchFilter
        {
            get { return _searchFilter; }
            set { SetProperty(ref _searchFilter, value); }
        }

        private string _notAddedPins = string.Empty;
        public string NotAddedPins
        {
            get { return _notAddedPins; }
            set { SetProperty(ref _notAddedPins, value); }
        }
        private ObservableCollection<PinGoogleMapModel> _pinsGoogleMapCollection;
        public ObservableCollection<PinGoogleMapModel> PinsGoogleMapCollection
        {
            get { return _pinsGoogleMapCollection; }
            set { SetProperty(ref _pinsGoogleMapCollection, value); }
        }

        private ICommand _addUpdatePinCommand;
        public ICommand AddUpdatePinCommand => _addUpdatePinCommand ??= new Command(OnAddUpdatePinCommandAsync);

        private ICommand _pinSelectedCommand;
        public ICommand PinSelectedCommand => _pinSelectedCommand ??= new Command<PinGoogleMapModel>(OnPinSelectedCommandAsync);

        private ICommand _exitCommand;
        public ICommand ExitCommand => _exitCommand ??= new Command(OnExitCommandAsync);

        private ICommand _addOrRemoveFavoritCommand;
        public ICommand AddOrRemoveFavoritCommand => _addOrRemoveFavoritCommand ??= new Command<PinGoogleMapModel>(OnAddOrRemoveFavoritPinCommandAsync);

        private ICommand _searchPinCommand;
        public ICommand SearchPinCommand => _searchPinCommand ??= new Command(OnSearchPinCommandAsync);

        private ICommand _editPinCommand;
        public ICommand EditPinCommand => _editPinCommand ??= new Command<PinGoogleMapModel>(OnEditPinSelectedCommandAsync);

        private ICommand _deletePinCommand;
        public ICommand DeletePinCommand => _deletePinCommand ??= new Command<PinGoogleMapModel>(OnDeletePinSelectedCommandAsync);
        #endregion

        private async void OnDeletePinSelectedCommandAsync(PinGoogleMapModel selectedPin)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Message = AppResources.AlertConfirmDelete,
                OkText = AppResources.AlertDelete,
                CancelText = AppResources.AlertCancel
            });

            if (result == true)
            {
                await _pinService.DeletePinAsync(selectedPin.Id);
                await InitTableAsync();
            }                            
        }        

        private async void OnEditPinSelectedCommandAsync(PinGoogleMapModel selectedPin)
        {
            NavigationParameters parametr = new NavigationParameters { { Constants.SELECT_PIN, selectedPin } };

            await _navigationService.NavigateAsync($"{nameof(AddUpdatePinPageView)}", parametr);
        }

        private async void OnSearchPinCommandAsync()
        {
            await InitTableAsync();
        }

        private async void OnAddOrRemoveFavoritPinCommandAsync(PinGoogleMapModel selectedPin)
        {
            selectedPin.IsFavorite = !selectedPin.IsFavorite;

            await _pinService.AddOrUpdatePinInDBAsync(selectedPin);

            await InitTableAsync();
        }

        private async void OnPinSelectedCommandAsync(PinGoogleMapModel selectedPin)
        {
            NavigationParameters parametr = new NavigationParameters { { Constants.SELECT_PIN, selectedPin } };

            await _navigationService.SelectTabAsync($"{nameof(MapPageView)}", parametr);            
        }

        private async void OnAddUpdatePinCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(AddUpdatePinPageView)}");
        }

        private async void OnExitCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(SignInPageView)}");

            _userAuthorization.ExitUser();
        }

        public async Task InitTableAsync()
        {
            var collection = await _pinService.GetPinsFromDBAsync(SearchFilter);

            PinsGoogleMapCollection = new ObservableCollection<PinGoogleMapModel>(collection);

            NotAddedPins = PinsGoogleMapCollection.Count == 0 ? "No pins added" : string.Empty;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {            
            await InitTableAsync();            
        }
    }
}
