using MapNotepad.Models;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.Sevices.Settings;
using System.Threading.Tasks;

namespace MapNotepad.Services.AuthenticationServices
{
    public class UserAuthorization : IUserAuthorization
    {
        private readonly IRepositoryService _repositoryService;
        private readonly ISettingsService _settingsService;

        private UserModel _currentUser;

        public UserAuthorization(IRepositoryService repositoryService, ISettingsService settingsService)
        {
            _repositoryService = repositoryService;
            _settingsService = settingsService;
        }

        public async Task<bool> CheckUserFromDBAsync(string email, string password)
        {
            bool isSuccess = false;

            _currentUser = await _repositoryService.GetItemAsync<UserModel>(us => us.Email == email.ToUpper() && us.Password == password);
            
            if (_currentUser != null)
            {
                isSuccess = true;
                _settingsService.IsAuthorized = true;
                _settingsService.CurrentUserID = GetIdCurrentUser();
            }

            return isSuccess;
        }

        public int GetIdCurrentUser()
        {
            return _currentUser.Id;
        }

        public void ExitUser()
        {
            _settingsService.IsAuthorized = false;
        }
    }
}