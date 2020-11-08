namespace MapNotepad.Sevices.Settings
{
    public interface ISettingsService
    {
        int CurrentUserID { get; set; }
        double MapLatitude { get; set; }
        double MapLongitude { get; set; }
        double MapZoom { get; set; }
    }
}
