using System.Text.RegularExpressions;

namespace MapNotepad.Services
{
    public class CheckPasswordValid : ICheckPasswordValid
    {
        public bool IsPasswordValid(string pasword)
        {
            bool isCapitalLetter = Regex.IsMatch(pasword, "[A-Z]{1}");
            bool isSmallLatter = Regex.IsMatch(pasword, "[a-z]{1}");
            bool isNumber = Regex.IsMatch(pasword, @"\d\w*");
            bool isErrorExist;

            isErrorExist = !isCapitalLetter; 

            if (isErrorExist == false) 
            {
                isErrorExist = !isSmallLatter; 
            }

            if (isErrorExist == false)
            {
                isErrorExist = !isNumber;   
            }

            if (isErrorExist == false) 
            {
                isErrorExist = pasword.Length < 8 || pasword.Length > 16; 
            }
            return isErrorExist;
        }
    }
}
