using MapNotepad.Models;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.Sevices.Settings;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System.Threading.Tasks;

namespace MapNotepad.Services.AuthenticationServices
{
    public class UserAuthorization : IUserAuthorization
    {
        private readonly IRepositoryService _repositoryService;
        private readonly ISettingsService _settingsService;

        private UserModel _currentUser;
        private ISettings UserSettings => CrossSettings.Current;

        public bool IsAuthorized
        {
            get => UserSettings.GetValueOrDefault(nameof(IsAuthorized), false);
            set => UserSettings.AddOrUpdateValue(nameof(IsAuthorized), value);
        }

        public UserAuthorization(IRepositoryService repositoryService, 
                                 ISettingsService settingsService)
        {
            _repositoryService = repositoryService;
            _settingsService = settingsService;
        }

        public async Task<bool> CheckUserFromDBAsync(string email, string password)
        {
            bool isSuccess = false;

            _currentUser = await _repositoryService.GetItemAsync<UserModel>(us => us.Email.ToUpper() == email.ToUpper() && us.Password == password);
            
            if (_currentUser != null)
            {
                isSuccess = true;
                IsAuthorized = true;
                _settingsService.CurrentUserEmail = GetEmailCurrentUser();
            }
            return isSuccess;
        }

        public string GetEmailCurrentUser()
        {
            return _currentUser.Email;
        }

        public void LogOut()
        {
            IsAuthorized = false;
        }
    }
}