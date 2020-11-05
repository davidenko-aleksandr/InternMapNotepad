
using System.Threading.Tasks;

namespace MapNotepad.Sevices.RegistrationServices
{
    public interface ICheckEmailValid
    {
        Task<bool> ValidateEmailInDBAsync(string email);
    }
}
