namespace MapNotepad.Sevices.MapPositionService
{
    public interface IMapPositionService
    {
        void ReadCameraPosition(double latitude, double longitude, double zoom);

        double Latitude();

        double Longitude();

        double Zoom();
    }
}