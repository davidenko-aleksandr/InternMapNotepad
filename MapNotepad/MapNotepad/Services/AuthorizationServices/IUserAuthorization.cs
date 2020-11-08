
using System.Threading.Tasks;

namespace MapNotepad.Services.AuthenticationServices
{
    public interface IUserAuthorization
    {
        bool IsAuthorized { get; set; }

        Task<bool> CheckUserFromDBAsync(string email, string password);

        int GetIdCurrentUser();

        void LogOut();
    }
}
