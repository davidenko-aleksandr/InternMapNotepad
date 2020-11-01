using MapNotepad.Services;
using System.Text.RegularExpressions;

namespace MapNotepad.Sevices.RegistrationServices
{
    public class CheckNameValid : ICheckNameValid
    {
        public bool ValidateName(string name)
        {
            string pattern = @"^\d\w*";

            return name.Length > 16 || Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase);
        }
    }
}
