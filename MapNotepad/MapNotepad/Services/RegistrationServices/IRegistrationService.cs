
using System.Threading.Tasks;

namespace MapNotepad.Sevices.RegistrationServices
{
    public interface IRegistrationService
    {
        Task<bool> ValidateEmailInDBAsync(string email);

        Task OnGoogleLogin();


    }
}
