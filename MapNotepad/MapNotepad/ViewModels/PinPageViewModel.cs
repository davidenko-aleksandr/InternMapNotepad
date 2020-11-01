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

namespace MapNotepad.ViewModels
{
    public class PinPageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IPinService _pinService;
        private readonly IUserAuthorization _userAuthorization;

        private ObservableCollection<PinGoogleMapModel> _pinsGoogleMapCollection;
        private ICommand _openAddUpdatePinPageCommand;
        private ICommand _pinSelectedCommand;
        private ICommand _exitCommand;
        private ICommand _addOrRemoveFavoritCommand;
        private ImageSource _imageFavorit;

        public ImageSource ImageFavorit
        {
            get { return _imageFavorit; }
            set { SetProperty(ref _imageFavorit, value); }
        }

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

        public ICommand OpenAddUpdatePinPageCommand => _openAddUpdatePinPageCommand ?? (_openAddUpdatePinPageCommand = new Command(
                                                       async () => await OpenAddUpdatePinPage()));

        public ICommand PinSelectedCommand => _pinSelectedCommand ?? (_pinSelectedCommand = new Command(
                                              async (Object obj) => await PinSelected(obj)));

        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new Command(
                                         async () => await ExitFromProfileAsync()));

        public ICommand AddOrRemoveFavoritCommand => _addOrRemoveFavoritCommand ?? (_addOrRemoveFavoritCommand = new Command(
                                                 async (Object obj) => await EditCurrentPin(obj)));

        private async Task EditCurrentPin(object obj)
        {
            if (obj is PinGoogleMapModel selectedPin)
            {
                selectedPin.IsFavorite = !selectedPin.IsFavorite;
                await _pinService.AddOrUpdatePinInDBAsync(selectedPin);
                //_ = InitTable();
            }
        }

        private async Task PinSelected(object obj)
        {
            if (obj is PinGoogleMapModel selectedPin)
            {
                NavigationParameters parametr = new NavigationParameters { { Constants.SELECT_PIN, selectedPin } };
                await _navigationService.SelectTabAsync(nameof(MapPageView), parametr);
            }
        }

        public async Task InitTable()    
        {
            var collection = await _pinService.GetPinsFromDBAsync();

            PinsGoogleMapCollection = new ObservableCollection<PinGoogleMapModel>(collection);
        }

        private async Task OpenAddUpdatePinPage()
        {
            await _navigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(AddUpdatePinPageView)}");
        }

        private async Task ExitFromProfileAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(SignInPageView)}");

            _userAuthorization.ExitUser();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            _ = InitTable();
        }
    }
}
