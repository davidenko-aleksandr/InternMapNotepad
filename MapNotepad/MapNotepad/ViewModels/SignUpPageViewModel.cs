using Prism.Navigation;
using Prism.Services;
using MapNotepad.Models;
using MapNotepad.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MapNotepad.Sevices.RegistrationServices;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.ViewModels;

namespace ProfileBook.ViewModels
{
    public class SignUpPageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _dialogService;
        private readonly ICheckPasswordValid _checkPasswordValid;
        private readonly ICheckEmailValid _checkEmailValid;
        private readonly IRepositoryService _repositoryService;
        private readonly ICheckNameValid _checkNameValid;

        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _conPassw = string.Empty;

        private ICommand _signUpCommand;

        public User User { get; set; }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

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

        public string ConPassw
        {
            get { return _conPassw; }
            set { SetProperty(ref _conPassw, value); }
        }

        public SignUpPageViewModel(INavigationService navigationService,
                                   IPageDialogService dialogService,
                                   ICheckPasswordValid checkPasswordValid,
                                   ICheckEmailValid checkEmailValid,
                                   IRepositoryService repositoryService,
                                   ICheckNameValid checkNameValid)
        {
            _checkPasswordValid = checkPasswordValid;
            _dialogService = dialogService;
            _checkEmailValid = checkEmailValid;
            _navigationService = navigationService;
            _repositoryService = repositoryService;
            _checkNameValid = checkNameValid;            
        }

        public ICommand SignUpCommand => _signUpCommand ?? (_signUpCommand = new Command(
                                        async () => await SignUpCompleteAsync(),
                                        () => false));

        private async Task SignUpCompleteAsync()
        {
            if (await ChekNameEmailPasswodAsync() == false)
            {
                await SaveToDataBase();

                var parametr = new NavigationParameters
                {
                    { "email", _email },
                    { "pas", _password }
                };

                await _navigationService.NavigateAsync(new System.Uri("http://www.ProfileBook/NavigationPage/SignInPageView", System.UriKind.Absolute), parametr);
            }
        }
        #region -- Checking fields for validity --
        private async Task<bool> ChekNameEmailPasswodAsync()
        {
            bool isErrorExist = false;
            
            if (_checkNameValid.IsCheckName(_name))
            {
                await _dialogService.DisplayAlertAsync("Incorrect name",
                    "name must not start with a number, " +
                    "name length must be no less than 1 characters " +
                    "and no more than 16 characters", "ok");   
                
                isErrorExist = true;
            }

            if (_checkEmailValid.IsCheckEmail(_email))
            {
                await _dialogService.DisplayAlertAsync("Incorrect login",
                    "E-mail address is incorrect ", "ok");

                Email = string.Empty;

                isErrorExist = true;
            }

            if (_checkPasswordValid.IsPasswordValid(_password))
            {
                await _dialogService.DisplayAlertAsync("Incorrect password",
                    "The password must contain from 8 to 16 characters, " +
                    "among which there must be a capital letter, " +
                    "a small letter, and also a number", "ok");

                Password = string.Empty;
                ConPassw = string.Empty;

                isErrorExist = true;
            }

            if (isErrorExist == false)
            {
                if (_password != _conPassw)
                {
                    await _dialogService.DisplayAlertAsync("Error",
                    "Password not confirmed", "ok");

                    Password = string.Empty;
                    ConPassw = string.Empty;

                    isErrorExist = true;
                }
            }

            if (isErrorExist == false)
            {
                bool isChekEmailDB = await _checkEmailValid.IsCheckEmailDB(_email);
                if (isChekEmailDB)
                {
                    await _dialogService.DisplayAlertAsync("Error",
                        "This login is already registered", "ok");

                    isErrorExist = true;
                }
            }

            return isErrorExist;
        }
        #endregion
        private async Task SaveToDataBase()
        {
            User user = new User
            {
                Name = _name,
                Email = _email.ToUpper(),
                Password = _password
            };
            
            await _repositoryService.SaveOrUpdateItemAsync<User>(user);
        }
    }
}