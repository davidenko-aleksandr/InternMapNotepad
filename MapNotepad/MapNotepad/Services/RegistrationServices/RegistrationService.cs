using MapNotepad.Models;
using MapNotepad.Services.AuthenticationServices;
using MapNotepad.Sevices.RegistrationServices;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.Sevices.Settings;
using MapNotepad.Views;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using Prism.Navigation;
using System.Threading.Tasks;

namespace MapNotepad.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IGoogleClientManager _googleService;
        private readonly ISettingsService _settingsService;
        private readonly IUserAuthorization _userAuthorization;
        public readonly INavigationService _navigationService;

        public string GoogleName { get; set; }
        public string GoogleEmail { get; set; }
        

        public RegistrationService(
                                   IRepositoryService repositoryService,
                                   ISettingsService settingsService,
                                   IUserAuthorization userAuthorization,
                                   INavigationService navigationService)
        {
            _repositoryService = repositoryService;
            _googleService = CrossGoogleClient.Current;
            _settingsService = settingsService;
            _userAuthorization = userAuthorization;
            _navigationService = navigationService;
        }

        public async Task<bool> ValidateEmailInDBAsync(string email)
        {
            var emailDB = await _repositoryService.GetItemAsync<UserModel>(us => us.Email.ToUpper() == email.ToUpper());

            return emailDB != null;
        }

        public async Task OnGoogleLogin()
        {
            _googleService.OnLogin += OnLoginCompleted;

            try
            {
                await _googleService.LoginAsync();
            }
            
            catch (GoogleClientBaseException e)
            {
                System.Diagnostics.Debug.WriteLine("Firebase Error", e.Message);
            }
        }

        public void OnLoginCompleted(object s, GoogleClientResultEventArgs<GoogleUser> loginEventArgs)
        {
            if (loginEventArgs != null)
            {
                GoogleUser googleUser = loginEventArgs.Data;

                _settingsService.CurrentUserEmail = googleUser.Email;

                _userAuthorization.IsAuthorized = true;
            }
            else
            {
                _navigationService.NavigateAsync($"{nameof(SignInPageView)}");
            }
            _googleService.OnLogin -= OnLoginCompleted;

            _googleService.Logout();
        }
    }
}
