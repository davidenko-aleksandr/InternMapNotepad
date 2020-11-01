using MapNotepad.Models;
using MapNotepad.Sevices.RegistrationServices;
using MapNotepad.Sevices.RepositoryService;
using System.Text.RegularExpressions;
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

        public bool ValidateEmailError(string email)
        {
            string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

            return _ = email.Length < 4
                     || email.Length > 16
                     || !Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        public async Task<bool> ValidateEmailInDBAsync(string email)
        {
            var emailDB = await _repositoryService.GetItemAsync<UserModel>(us => us.Email.ToUpper() == email.ToUpper());

            return emailDB != null;
        }
    }
}
