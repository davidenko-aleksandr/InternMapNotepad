namespace MapNotepad.Sevices.Settings
{
    public interface ISettingsService
    {
        bool IsAuthorized { get; set; }
        int CurrentUserID { get; set; }
        double MapLatitude { get; set; }
        double MapLongitude { get; set; }
        public double MapZoom { get; set; }
    }
}
