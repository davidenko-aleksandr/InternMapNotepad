using MapNotepad.Models;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MapNotepad.Sevices.PinServices;
using System;
using MapNotepad.Views;
using Prism.Navigation.TabbedPages;
using MapNotepad.Services.AuthenticationServices;
using Acr.UserDialogs;
using MapNotepad.Resources;

namespace MapNotepad.ViewModels
{
    public class PinPageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IPinService _pinService;
        private readonly IUserAuthorization _userAuthorization;
        
        private ICommand _openAddUpdatePinPageCommand;
        private ICommand _pinSelectedCommand;
        private ICommand _exitCommand;
        private ICommand _addOrRemoveFavoritCommand;
        private ICommand _searchPinCommand;
        private ICommand _editPinCommand;
        private ICommand _deletePinCommand;

        private ImageSource _imageFavorit = (ImageSource)"no_favorite.png";

        public ImageSource ImageFavorit
        {
            get { return _imageFavorit; }
            set { SetProperty(ref _imageFavorit, value); }
        }

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

        public PinPageViewModel(INavigationService navigationService, 
                                IPinService pinService,
                                IUserAuthorization userAuthorization)
        {
            _navigationService = navigationService;
            _pinService = pinService;
            _userAuthorization = userAuthorization;            
        }       

        public ICommand OpenAddUpdatePinPageCommand => _openAddUpdatePinPageCommand ??= new Command(async () => await OpenAddUpdatePinPageAsync());

        public ICommand PinSelectedCommand => _pinSelectedCommand ??= new Command(async (Object obj) => await PinSelectedAsync(obj));

        public ICommand ExitCommand => _exitCommand ??= new Command(async () => await ExitFromProfileAsync());

        public ICommand AddOrRemoveFavoritCommand => _addOrRemoveFavoritCommand ??= new Command(async (Object obj) => await AddOrRemoveFavoritPinAsync(obj));

        public ICommand SearchPinCommand => _searchPinCommand ??= new Command(async (Object obj) => await SearchPin(obj));

        public ICommand EditPinCommand => _editPinCommand ??= new Command(async (Object obj) => await EditPinSelectedAsync(obj));

        public ICommand DeletePinCommand => _deletePinCommand ??= new Command(async (Object obj) => await DeletePinSelectedAsync(obj));

        private async Task DeletePinSelectedAsync(object obj)
        {
            if (obj is PinGoogleMapModel selectedPin)
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
        }        

        private async Task EditPinSelectedAsync(object obj)
        {
            if (obj is PinGoogleMapModel selectedPin)
            {
                NavigationParameters parametr = new NavigationParameters { { Constants.SELECT_PIN, selectedPin } };
                await _navigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(AddUpdatePinPageView)}", parametr);
            }
        }

        private Task SearchPin(object obj)
        {
            if (obj is string filt)
            {
                SearchFilter = filt;
              return  InitTableAsync();
            }
            if (obj == null)
            {
                return InitTableAsync();
            }
            else return InitTableAsync();
        }

        private async Task AddOrRemoveFavoritPinAsync(object obj)
        {
            if (obj is PinGoogleMapModel selectedPin)
            {
                selectedPin.IsFavorite = !selectedPin.IsFavorite;

                await _pinService.AddOrUpdatePinInDBAsync(selectedPin);

                await InitTableAsync();
            }
        }

        private async Task PinSelectedAsync(object obj)
        {
            if (obj is PinGoogleMapModel selectedPin)
            {
                NavigationParameters parametr = new NavigationParameters { { Constants.SELECT_PIN, selectedPin } };

                await _navigationService.SelectTabAsync($"{nameof(MapPageView)}", parametr);
            }
        }

        public async Task InitTableAsync()    
        {
            var collection = await _pinService.GetPinsFromDBAsync(SearchFilter);

            PinsGoogleMapCollection = new ObservableCollection<PinGoogleMapModel>(collection);

            NotAddedPins = PinsGoogleMapCollection.Count == 0 ? "No pins added" : string.Empty;
        }

        private async Task OpenAddUpdatePinPageAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(AddUpdatePinPageView)}");
        }

        private async Task ExitFromProfileAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(SignInPageView)}");

            _userAuthorization.ExitUser();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {            
            await InitTableAsync();            
        }
    }
}
