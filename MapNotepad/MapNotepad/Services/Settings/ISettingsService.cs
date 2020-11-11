namespace MapNotepad.Sevices.Settings
{
    public interface ISettingsService
    {
        string CurrentUserEmail { get; set; }

        double MapLatitude { get; set; }

        double MapLongitude { get; set; }

        double MapZoom { get; set; }
    }
}
