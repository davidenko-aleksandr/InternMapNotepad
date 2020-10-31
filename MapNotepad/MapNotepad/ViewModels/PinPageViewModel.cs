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

        private ObservableCollection<PinGoogleMap> _pinsGoogleMapCollection;
        private ICommand _openAddUpdatePinPageCommand;
        private ICommand _pinSelectedCommand;
        private ICommand _exitCommand;
        // private ICommand _addOrRemoveFavoritCommand;
        private ImageSource _imageFavorit;

        public ImageSource ImageFavorit
        {
            get { return _imageFavorit; }
            set { SetProperty(ref _imageFavorit, value); }
        }

        public ObservableCollection<PinGoogleMap> PinsGoogleMapCollection
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

     //   public ICommand AddOrRemoveFavoritCommand => _addOrRemoveFavoritCommand ?? (_openAddUpdatePinPageCommand = new Command(ChangeImage));

        //private void ChangeImage(object obj)
        //{
            //if (ImageFavorit == "no_favorite.png")
            //{
            //    ImageFavorit = "favorite.png";
            //}
            //else
            //{
            //    ImageFavorit = "no_favorite.png";
            //}            
      //  }

        public ICommand OpenAddUpdatePinPageCommand => _openAddUpdatePinPageCommand ?? (_openAddUpdatePinPageCommand = new Command(
                                                       async () => await OpenAddUpdatePinPage()));

        public ICommand PinSelectedCommand => _pinSelectedCommand ?? (_pinSelectedCommand = new Command(
                                              async (Object obj) => await PinSelected(obj)));
        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new Command(
                                         async () => await ExitFromProfileAsync()));
        private async Task PinSelected(object obj)
        {
            PinGoogleMap selectedPin = (PinGoogleMap)obj;

            var parametr = new NavigationParameters
            {
                {"selectedPin", selectedPin }
            };

            await _navigationService.SelectTabAsync(nameof(MapPageView), parametr);
        }

        public async Task InitTable()    
        {
            var collection = await _pinService.GetPinsFromDBAsync();

            PinsGoogleMapCollection = new ObservableCollection<PinGoogleMap>(collection);
        }

        private async Task OpenAddUpdatePinPage()
        {
            await _navigationService.NavigateAsync("NavigationPage/AddUpdatePinPageView");
        }

        private async Task ExitFromProfileAsync()
        {
            await _navigationService.NavigateAsync(new Uri("http://WWW.ProfileBook/NavigationPage/SignInPageView", UriKind.Absolute));

            _userAuthorization.UserExit();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            _ = InitTable();
        }
    }
}
