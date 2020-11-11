using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MapNotepad.Sevices.Settings
{
    public class SettingsService : ISettingsService
    {
        private  ISettings UserSettings => CrossSettings.Current;

        public string CurrentUserEmail
        {
            get => UserSettings.GetValueOrDefault(nameof(CurrentUserEmail), "0");
            set => UserSettings.AddOrUpdateValue(nameof(CurrentUserEmail), value);
        }

        public double MapLatitude
        {
            get => UserSettings.GetValueOrDefault(nameof(MapLatitude), 48.4536560005801);
            set => UserSettings.AddOrUpdateValue(nameof(MapLatitude), value);
        }
        public double MapLongitude
        {
            get => UserSettings.GetValueOrDefault(nameof(MapLongitude), 35.0374626740813);
            set => UserSettings.AddOrUpdateValue(nameof(MapLongitude), value);
        }
        public double MapZoom
        {
            get => UserSettings.GetValueOrDefault(nameof(MapZoom), 15.0);
            set => UserSettings.AddOrUpdateValue(nameof(MapZoom), value);
        }
    }
}