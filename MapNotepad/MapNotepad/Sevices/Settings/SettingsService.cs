using Plugin.Settings;
using Plugin.Settings.Abstractions;


namespace MapNotepad.Sevices.Settings
{
    public class SettingsService : ISettingsService
    {
        private  ISettings UserSettings => CrossSettings.Current;

        public bool IsAuthorized
        {
            get => UserSettings.GetValueOrDefault(nameof(IsAuthorized), false);
            set => UserSettings.AddOrUpdateValue(nameof(IsAuthorized), value);
        }

        public int CurrentUserID
        {
            get => UserSettings.GetValueOrDefault(nameof(CurrentUserID), 0);
            set => UserSettings.AddOrUpdateValue(nameof(CurrentUserID), value);
        }
    }
}