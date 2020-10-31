namespace MapNotepad.Sevices.Settings
{
    public interface ISettingsService
    {
        bool IsAuthorized { get; set; }
        int CurrentUserID { get; set; }
    }
}
