using MapNotepad.Models;
using MapNotepad.Services.AuthenticationServices;
using MapNotepad.Sevices.RegistrationServices;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.Sevices.Settings;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System.Threading.Tasks;

namespace MapNotepad.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IGoogleClientManager _googleService;
        private readonly ISettingsService _settingsService;
        private readonly IUserAuthorization _userAuthorization;

        public string GoogleName { get; set; }
        public string GoogleEmail { get; set; }
        

        public RegistrationService(
                                   IRepositoryService repositoryService,
                                   ISettingsService settingsService,
                                   IUserAuthorization userAuthorization)
        {
            _repositoryService = repositoryService;
            _googleService = CrossGoogleClient.Current;
            _settingsService = settingsService;
            _userAuthorization = userAuthorization;
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
            if (loginEventArgs.Data != null)
            {
                GoogleUser googleUser = loginEventArgs.Data;

                _settingsService.CurrentUserEmail = googleUser.Email;

                _userAuthorization.IsAuthorized = true;
            }
            _googleService.OnLogin -= OnLoginCompleted;

            _googleService.Logout();
        }
    }
}
