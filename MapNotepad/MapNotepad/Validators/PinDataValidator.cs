
namespace MapNotepad.Validators
{
    public class PinDataValidator
    {
        public static bool ValidateLatitude(double latitude)
        {
            return latitude > -90 && latitude < 90;
        }

        public static bool ValidateLongitude(double longitude)
        {
            return longitude > -180 && longitude < 180;
        }
    }
}
