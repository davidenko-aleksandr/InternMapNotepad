using Plugin.Settings;
using Plugin.Settings.Abstractions;


namespace MapNotepad.Sevices.Settings
{
    public class SettingsService : ISettingsService
    {
        private  ISettings AppSettings => CrossSettings.Current;

        public bool IsAuthorized
        {
            get => AppSettings.GetValueOrDefault(nameof(IsAuthorized), false);
            set => AppSettings.AddOrUpdateValue(nameof(IsAuthorized), value);
        }

        public int CurrentUserID
        {
            get => AppSettings.GetValueOrDefault(nameof(CurrentUserID), 0);
            set => AppSettings.AddOrUpdateValue(nameof(CurrentUserID), value);
        }
    }
}