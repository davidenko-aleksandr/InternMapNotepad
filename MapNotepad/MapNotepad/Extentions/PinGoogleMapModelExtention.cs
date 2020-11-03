using MapNotepad.Models;
using Xamarin.Forms.GoogleMaps;

namespace MapNotepad.Extentions
{
    public static class PinGoogleMapModelExtention
    {
        public static Pin ToPin(this PinGoogleMapModel pin)
        {
            return new Pin
            {
                Label = pin.Label,
                Position = new Position(pin.Latitude, pin.Longitude),
                Address = pin.Description

            };
        }
    }
}
