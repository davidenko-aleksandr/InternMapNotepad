
using System.Threading.Tasks;

namespace MapNotepad.Sevices.RegistrationServices
{
    public interface ICheckEmailValid
    {
        bool ValidateEmailError(string login);
        Task<bool> ValidateEmailInDBAsync(string email);
    }
}
