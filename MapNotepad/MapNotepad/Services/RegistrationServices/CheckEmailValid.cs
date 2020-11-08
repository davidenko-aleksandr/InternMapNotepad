using MapNotepad.Models;
using MapNotepad.Sevices.RegistrationServices;
using MapNotepad.Sevices.RepositoryService;
using System.Threading.Tasks;

namespace MapNotepad.Services
{
    public class CheckEmailValid : ICheckEmailValid
    {
        private readonly IRepositoryService _repositoryService; 

        public CheckEmailValid(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        public async Task<bool> ValidateEmailInDBAsync(string email)
        {
            var emailDB = await _repositoryService.GetItemAsync<UserModel>(us => us.Email.ToUpper() == email.ToUpper());

            return emailDB != null;
        }
    }
}
