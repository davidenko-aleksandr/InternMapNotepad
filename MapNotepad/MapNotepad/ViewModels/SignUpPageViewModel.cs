using Prism.Navigation;
using Prism.Services;
using MapNotepad.Models;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using MapNotepad.Sevices.RegistrationServices;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.ViewModels;
using MapNotepad.Views;
using MapNotepad.Resources;
using MapNotepad.Validators;

namespace ProfileBook.ViewModels
{
    public class SignUpPageViewModel : BaseViewModel
    {
        private readonly IPageDialogService _dialogService;
        private readonly ICheckEmailValid _checkEmailValid;
        private readonly IRepositoryService _repositoryService;      

        public SignUpPageViewModel(
                                   INavigationService navigationService,
                                   IPageDialogService dialogService,
                                   ICheckEmailValid checkEmailValid,
                                   IRepositoryService repositoryService) : base(navigationService)
        {
            _dialogService = dialogService;
            _checkEmailValid = checkEmailValid;
            _repositoryService = repositoryService;  
        }

        #region -- Public properties --

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

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { SetProperty(ref _confirmPassword, value); }
        }
        private ICommand _signUpCommand;
        public ICommand SignUpCommand => _signUpCommand ??= new Command( async () => await OnSignUpCommandAsync(), () => false);

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new Command(OnBackCommandAsync);

        #endregion

        private async void OnBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private async Task OnSignUpCommandAsync()
        {
            bool chekRegistrData = await ChekNameEmailPasswodAsync();

            if (!chekRegistrData)
            {
                await SaveUserToDBAsync();

                var parameters = new NavigationParameters
                {
                    { "email", _email },
                    { "pas", _password }
                };

                await _navigationService.NavigateAsync($"{nameof(SignInPageView)}", parameters);
            }
        }

        #region -- Checking fields for validity --

        private async Task<bool> ChekNameEmailPasswodAsync()
        {
            bool isErrorExist = false;

            if (UserDataValidator.ValidateName(_name))
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertIncorrectName, AppResources.AlertNameRequared, AppResources.AlertOk);   
                
                isErrorExist = true;
            }
            
            if (UserDataValidator.ValidateEmail(_email))
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertIncorrectEmail, AppResources.AlertEmailRequared, AppResources.AlertOk);

                Email = string.Empty;

                isErrorExist = true;
            }

            if (UserDataValidator.ValidatePassword(_password))
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertIncorrectPassword, AppResources.AlertPasswordRequared, AppResources.AlertOk);

                Password = string.Empty;
                ConfirmPassword = string.Empty;

                isErrorExist = true;
            }

            if (!isErrorExist && _password != _confirmPassword)
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertError, AppResources.AlertNotConfirmPassword, AppResources.AlertOk);

                Password = string.Empty;
                ConfirmPassword = string.Empty;

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