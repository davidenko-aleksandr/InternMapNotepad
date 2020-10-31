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

        public bool IsCheckEmail(string login)
        {
            bool isInvalidLOgin = false;
            string pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

            if (login.Length < 4 ||
                login.Length > 16 ||
                !Regex.IsMatch(login, pattern, RegexOptions.IgnoreCase))
            {
                isInvalidLOgin = true;
            }
            return isInvalidLOgin;
        }

        public async Task<bool> IsCheckEmailDB(string email)
        {
            var emailDB = await _repositoryService.GetItemAsync<User>(us => us.Email == email.ToUpper());

            return emailDB != null;
        }
    }
}
