using MapNotepad.Services;
using MapNotepad.Services.AuthenticationServices;
using MapNotepad.Services.NoteService;
using MapNotepad.Sevices.MapPositionService;
using MapNotepad.Sevices.PermissionServices;
using MapNotepad.Sevices.PinServices;
using MapNotepad.Sevices.RegistrationServices;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.Sevices.Settings;
using MapNotepad.ViewModels;
using MapNotepad.ViewModels.PopupPageViewModels;
using MapNotepad.Views;
using MapNotepad.Views.PopupPageViews;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Plugin.Popups;
using ProfileBook.ViewModels;
using Xamarin.Forms;

namespace MapNotepad
{
    public partial class App : PrismApplication
    {   
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterForNavigation<NavigationPage>();

            //registration of pages and view models
            containerRegistry.RegisterForNavigation<SignInPageView, SignInPageViewModel>();
            containerRegistry.RegisterForNavigation<SignUpPageView, SignUpPageViewModel>();    
            containerRegistry.RegisterForNavigation<AddUpdatePinPageView, AddUpdatePinPageViewModel>();
            containerRegistry.RegisterForNavigation<MainTabbedPageView>();
            containerRegistry.RegisterForNavigation<MapPageView, MapPageViewModel>();
            containerRegistry.RegisterForNavigation<PinPageView, PinPageViewModel>();
            containerRegistry.RegisterForNavigation<AddNotePageView, AddNotePageViewModel>();
            containerRegistry.RegisterForNavigation<ListOfNotesPageView, ListOfNotesPageViewModel>();            

            //registration of services with interfaces
            containerRegistry.RegisterInstance<IRepositoryService>(Container.Resolve<RepositoryService>());
            containerRegistry.RegisterInstance<ISettingsService>(Container.Resolve<SettingsService>());
            containerRegistry.RegisterInstance<ICheckEmailValid>(Container.Resolve<CheckEmailValid>());
            containerRegistry.RegisterInstance<IUserAuthorization>(Container.Resolve<UserAuthorization>());
            containerRegistry.RegisterInstance<IPermissionService>(Container.Resolve<PermissionService>());
            containerRegistry.RegisterInstance<IPinService>(Container.Resolve<PinService>());
            containerRegistry.RegisterInstance<IMapPositionService>(Container.Resolve<MapPositionService>());
            containerRegistry.RegisterInstance<INoteForPinService>(Container.Resolve<NoteForPinService>());
            
        }

        public static T Resolve<T>()
        {
            return Current.Container.Resolve<T>();
        }

        protected override void OnInitialized()
        {
            Device.SetFlags(new string[] { "RadioButton_Experimental" });

            if (Resolve<IUserAuthorization>().IsAuthorized)
            {
                NavigationService.NavigateAsync($"{nameof(MainTabbedPageView)}?selectedTab={nameof(MapPageView)}");
            }
            else
            {                
                NavigationService.NavigateAsync($"{nameof(SignInPageView)}");
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
