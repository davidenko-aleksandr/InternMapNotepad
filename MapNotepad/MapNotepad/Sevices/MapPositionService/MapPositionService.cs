using Android.Print;
using MapNotepad.Sevices.Settings;

namespace MapNotepad.Sevices.MapPositionService
{
    public class MapPositionService : IMapPositionService
    {
        private readonly ISettingsService _settingsService;

        public MapPositionService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void ReadCameraPosition(double latitude, double longitude, double zoom)
        {
            _settingsService.MapLatitude = latitude;
            _settingsService.MapLongitude = longitude;
            _settingsService.MapZoom = zoom;

        }

        public double Latitude()
        {
            return _settingsService.MapLatitude;            
        }

        public double Longitude()
        {
            return _settingsService.MapLongitude;
        }

        public double Zoom()
        {
            return _settingsService.MapZoom;
        }
    }
}
