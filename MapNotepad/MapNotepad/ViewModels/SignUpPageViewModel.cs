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
using MapNotepad.Views;
using MapNotepad.Resources;

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

        private ICommand _signUpCommand;       

        public UserModel User { get; set; }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

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

        private string _conPassw = string.Empty;
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

        public ICommand SignUpCommand => _signUpCommand ??= new Command( async () => await SignUpCompleteAsync(), () => false);

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new Command( async () => await ComeBackAsync());

        private async Task ComeBackAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private async Task SignUpCompleteAsync()
        {
            bool chekRegistrData = await ChekNameEmailPasswodAsync();

            if (!chekRegistrData)
            {
                await SaveUserToDBAsync();

                var parametr = new NavigationParameters
                {
                    { "email", _email },
                    { "pas", _password }
                };

                await _navigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(SignInPageView)}", parametr);
            }
        }

        #region -- Checking fields for validity --
        private async Task<bool> ChekNameEmailPasswodAsync()
        {
            bool isErrorExist = false;

            if (_checkNameValid.ValidateName(_name))
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertIncorrectName, AppResources.AlertNameRequared, AppResources.AlertOk);   
                
                isErrorExist = true;
            }

            if (_checkEmailValid.ValidateEmailError(_email))
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertIncorrectEmail, AppResources.AlertEmailRequared, AppResources.AlertOk);

                Email = string.Empty;

                isErrorExist = true;
            }

            if (_checkPasswordValid.PasswordValidation(_password))
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertIncorrectPassword, AppResources.AlertPasswordRequared, AppResources.AlertOk);

                Password = string.Empty;
                ConPassw = string.Empty;

                isErrorExist = true;
            }

            if (!isErrorExist && _password != _conPassw)
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertError, AppResources.AlertNotConfirmPassword, AppResources.AlertOk);

                Password = string.Empty;
                ConPassw = string.Empty;

                isErrorExist = true;                
            }

            if (!isErrorExist)
            {
                bool isChekEmailDB = await _checkEmailValid.ValidateEmailInDBAsync(_email);

                if (isChekEmailDB)
                {
                    await _dialogService.DisplayAlertAsync(AppResources.AlertError, AppResources.AlertEmailAlredyRegistred, AppResources.AlertOk);

                    isErrorExist = true;
                }
            }

            return isErrorExist;
        }
        #endregion
        private async Task SaveUserToDBAsync()
        {
            UserModel user = new UserModel
            {
                Name = _name,
                Email = _email.ToUpper(),
                Password = _password
            };
            
            await _repositoryService.SaveOrUpdateItemAsync<UserModel>(user);
        }
    }
}