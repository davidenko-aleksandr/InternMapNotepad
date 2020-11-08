using Prism.Navigation;
using Prism.Services;
using MapNotepad.Models;
using MapNotepad.Services.AuthenticationServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MapNotepad.ViewModels;
using MapNotepad.Sevices.PermissionServices;
using MapNotepad.Views;
using MapNotepad.Resources;

namespace ProfileBook.ViewModels
{
    public class SignInPageViewModel : BaseViewModel
    {
        private readonly IUserAuthorization _userAuthentication;
        private readonly IPageDialogService _dialogService;
        private readonly IPermissionService _permissionService;

        public SignInPageViewModel(
                                   INavigationService navigationService,
                                   IUserAuthorization userAuthentication,
                                   IPageDialogService dialogService,
                                   IPermissionService permissionService) : base(navigationService)
        {
            _userAuthentication = userAuthentication;
            _dialogService = dialogService;
            _permissionService = permissionService;
        }
        #region -- Public properties --
        public UserModel User { get; set; }

        private string _email = string.Empty;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private ICommand _enterCommand;        
        public ICommand EnterCommand => _enterCommand ??= new Command( async () => await OnEnterCommandAsync(), () => false);

        private ICommand _toSignUpPageCommand;
        public ICommand ToSignUpPageCommand => _toSignUpPageCommand ??= new Command(OnToSignUpPageCommandAsync);

        #endregion

        private async void OnToSignUpPageCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(SignUpPageView)}");
        }

        private async Task OnEnterCommandAsync()
        {
            var isUserValid = await _userAuthentication.CheckUserFromDBAsync(_email, _password);

            if (isUserValid)
            {
                await _permissionService.PermissionRequestAsync();

                await _navigationService.NavigateAsync($"{nameof(MainTabbedPageView)}?selectedTab={nameof(MapPageView)}");
            }
            else
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertError, AppResources.AlertInvalidLoginOrPassword, AppResources.AlertOk);
                Email = string.Empty;
                Password = string.Empty;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Email = (string)parameters["email"];
            Password = (string)parameters["pas"];
        }
    }
}
