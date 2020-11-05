﻿using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Prism.Ioc;
using Prism;
using Android;
using Plugin.Permissions;
using Acr.UserDialogs;

namespace MapNotepad.Droid
{
    [Activity(Label = "MapNotepad", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int RequestLocationId = 0;

        readonly string[] LocationPermissions =
        {
        Manifest.Permission.AccessCoarseLocation,
        Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            TabLayoutResource = Resource.Layout.Tabbar;

            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            UserDialogs.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState);

            LoadApplication(new App(new AndroidInitializer()));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnStart()
        {
            base.OnStart();

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(LocationPermissions, RequestLocationId);
                }
                else
                {
                    // Permissions already granted - display a message.
                }
            }
        }
        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}