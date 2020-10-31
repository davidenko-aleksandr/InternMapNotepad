
using System.Threading.Tasks;

namespace MapNotepad.Sevices.RegistrationServices
{
    public interface ICheckEmailValid
    {
        bool IsCheckEmail(string login);
        Task<bool> IsCheckEmailDB(string email);
    }
}
