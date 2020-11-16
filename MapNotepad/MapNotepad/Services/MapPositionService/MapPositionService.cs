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

        public double Latitude
        {
            get { return _settingsService.MapLatitude; }
        }

        public double Longitude
        {
            get { return _settingsService.MapLongitude; }
        }

        public double Zoom
        {
            get { return _settingsService.MapZoom; }
        }

        public void ReadCameraPosition(double latitude, double longitude, double zoom)
        {
            _settingsService.MapLatitude = latitude;
            _settingsService.MapLongitude = longitude;
            _settingsService.MapZoom = zoom;
        }
    }
}
