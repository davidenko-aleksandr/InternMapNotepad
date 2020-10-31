using MapNotepad.Services;
using System.Text.RegularExpressions;

namespace MapNotepad.Sevices.RegistrationServices
{
    public class CheckNameValid : ICheckNameValid
    {
        public bool IsCheckName(string name)
        {
            bool isInvalidLOgin = false;
            string pattern = @"^\d\w*";

            if (name.Length < 1 ||
                name.Length > 16 ||
                Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase))
            {
                isInvalidLOgin = true;
            }
            return isInvalidLOgin;
        }
    }
}
