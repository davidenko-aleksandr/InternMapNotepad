using System.Text.RegularExpressions;

namespace MapNotepad.Validators
{
    public class UserDataValidator
    {
        public const string EMAIL_VALID = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        public const string NAME_VALID = @"^\d\w*";
        public const string PASSWORD_UP_VALID = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$";

        public static bool ValidateEmail(string email)
        {
            return email.Length < 4
                   || email.Length > 30
                   || !Regex.IsMatch(email, EMAIL_VALID, RegexOptions.IgnoreCase);
        }

        public static bool ValidateName(string name)
        {
            return name.Length > 30 || Regex.IsMatch(name, NAME_VALID, RegexOptions.IgnoreCase);
        }

        public static bool ValidatePassword(string pasword)
        {
            return !Regex.IsMatch(pasword, PASSWORD_UP_VALID, RegexOptions.IgnoreCase);
        }
    }
}
