
using System.Threading.Tasks;

namespace MapNotepad.Services.AuthenticationServices
{
    public interface IUserAuthorization
    {
        Task<bool> CheckUserFromDBAsync(string email, string password);

        int GetIdCurrentUser();

        void UserExit();
    }
}
