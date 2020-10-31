using Prism.Navigation;
using Prism.Services;
using MapNotepad.Models;
using MapNotepad.Services.AuthenticationServices;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MapNotepad.ViewModels;
using MapNotepad.Sevices.PermissionServices;

namespace ProfileBook.ViewModels
{
    public class SignInPageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IUserAuthorization _userAuthentication;
        private readonly IPageDialogService _dialogService;
        private readonly IPermissionService _permissionService;

        private string _email = string.Empty;
        private string _password = string.Empty;

        private ICommand _enterCommand;
        private ICommand _toSignUpPageCommand;

        public User User { get; set; }

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public SignInPageViewModel(INavigationService navigationService,
                                    IUserAuthorization userAuthentication,
                                    IPageDialogService dialogService,
                                    IPermissionService permissionService)
        {
            _navigationService = navigationService;
            _userAuthentication = userAuthentication;
            _dialogService = dialogService;
            _permissionService = permissionService;
        }

        public ICommand EnterCommand => _enterCommand ?? (_enterCommand = new Command(
                        async () => await OpenMainTabbedPageAsync(),
                        () => false));

        public ICommand ToSignUpPageCommand => _toSignUpPageCommand ?? (_toSignUpPageCommand = new Command(
                        async () => await OpenSignUpPageAsync()));

        async Task OpenSignUpPageAsync()
        {
            await _navigationService.NavigateAsync("SignUpPageView");
        }

        async Task OpenMainTabbedPageAsync()
        {
            var isUserValid = await _userAuthentication.CheckUserFromDBAsync(_email, _password);

            if (isUserValid)
            {
                await _permissionService.PermissionRequestAsync();

                await _navigationService.NavigateAsync(new Uri("http://WWW.ProfileBook/NavigationPage/MainTabbedPageView", UriKind.Absolute));
            }
            else
            {
                await _dialogService.DisplayAlertAsync("Error", "Invalid login or password", "Ok");
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
