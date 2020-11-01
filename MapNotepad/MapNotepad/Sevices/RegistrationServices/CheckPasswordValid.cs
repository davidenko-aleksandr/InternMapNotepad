using System.Text.RegularExpressions;

namespace MapNotepad.Services
{
    public class CheckPasswordValid : ICheckPasswordValid
    {
        public bool PasswordValidation(string pasword)
        {
            bool isCapitalLetter = Regex.IsMatch(pasword, "[A-Z]{1}");
            bool isSmallLatter = Regex.IsMatch(pasword, "[a-z]{1}");
            bool isNumber = Regex.IsMatch(pasword, @"\d\w*");

            return !isCapitalLetter &&
                   !isSmallLatter &&
                   !isNumber &&
                   (pasword.Length < 8 || pasword.Length > 16);
        }
    }
}
