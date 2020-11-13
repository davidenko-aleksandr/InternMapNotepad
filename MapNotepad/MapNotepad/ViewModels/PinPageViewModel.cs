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
using MapNotepad.Services.NoteService;

namespace MapNotepad.ViewModels
{
    public class PinPageViewModel : BaseViewModel
    {
        private readonly IPinService _pinService;
        private readonly IUserAuthorization _userAuthorization;
        private readonly INoteForPinService _noteForPinService;
        
        public PinPageViewModel(
                                INavigationService navigationService,
                                IPinService pinService,
                                INoteForPinService noteForPinService,
                                IUserAuthorization userAuthorization) : base(navigationService)
        {
            _pinService = pinService;
            _userAuthorization = userAuthorization;
            _noteForPinService = noteForPinService;
        }

        #region -- Public properties --

        private string _searchFilter = string.Empty;
        public string SearchFilter
        {
            get { return _searchFilter; }
            set { SetProperty(ref _searchFilter, value); }
        }

        private string _noPins = string.Empty;
        public string NoPins
        {
            get { return _noPins; }
            set { SetProperty(ref _noPins, value); }
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

        private ICommand _openNotesCommand;
        public ICommand OpenNotesCommand => _openNotesCommand ??= new Command<PinGoogleMapModel>(OnOpenNotesCommandAsync);

        private async void OnOpenNotesCommandAsync(PinGoogleMapModel pinModel)
        {
            NavigationParameters parameters = new NavigationParameters { { Constants.SELECTED_PIN, pinModel.Id } };

            await _navigationService.NavigateAsync($"{nameof(ListOfNotesPageView)}", parameters); 
        }

        #endregion

        #region -- Private helpers --

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
                await _noteForPinService.DeleteCollectionNoteAsync(selectedPin.Id);

                await _pinService.DeletePinAsync(selectedPin.Id);                

                await InitTableAsync();
            }                            
        }        

        private async void OnEditPinSelectedCommandAsync(PinGoogleMapModel selectedPin)
        {
            NavigationParameters parameters = new NavigationParameters { { Constants.SELECTED_PIN, selectedPin } };

            await _navigationService.NavigateAsync($"{nameof(AddUpdatePinPageView)}", parameters);
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
            NavigationParameters parametr = new NavigationParameters { { Constants.SELECTED_PIN, selectedPin } };

            await _navigationService.SelectTabAsync($"{nameof(MapPageView)}", parametr);            
        }

        private async void OnAddUpdatePinCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(AddUpdatePinPageView)}");
        }

        private async void OnExitCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(SignInPageView)}");

            _userAuthorization.LogOut();
        }

        public async Task InitTableAsync()
        {
            var collection = await _pinService.GetPinsFromDBAsync(SearchFilter);

            PinsGoogleMapCollection = new ObservableCollection<PinGoogleMapModel>(collection);

            NoPins = PinsGoogleMapCollection.Count == 0 ? "No pins added" : string.Empty;
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {            
            await InitTableAsync();            
        }

        #endregion
    }
}
