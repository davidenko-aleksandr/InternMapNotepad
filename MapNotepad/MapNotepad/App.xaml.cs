using MapNotepad.Services;
using MapNotepad.Services.AuthenticationServices;
using MapNotepad.Sevices.PermissionServices;
using MapNotepad.Sevices.PinServices;
using MapNotepad.Sevices.RegistrationServices;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.Sevices.Settings;
using MapNotepad.ViewModels;
using MapNotepad.Views;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using ProfileBook.ViewModels;
using Xamarin.Forms;

namespace MapNotepad
{
    public partial class App : PrismApplication
    {   
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();

            //registration of pages and view models
            containerRegistry.RegisterForNavigation<SignInPageView, SignInPageViewModel>();
            containerRegistry.RegisterForNavigation<SignUpPageView, SignUpPageViewModel>();    
            containerRegistry.RegisterForNavigation<AddUpdatePinPageView, AddUpdatePinPageViewModel>();
            containerRegistry.RegisterForNavigation<MainTabbedPageView>();
            containerRegistry.RegisterForNavigation<MapPageView, MapPageViewModel>();
            containerRegistry.RegisterForNavigation<PinPageView, PinPageViewModel>();          

            //registration of services with interfaces
            containerRegistry.RegisterInstance<IRepositoryService>(Container.Resolve<RepositoryService>());
            containerRegistry.RegisterInstance<ISettingsService>(Container.Resolve<SettingsService>());
            containerRegistry.RegisterInstance<ICheckPasswordValid>(Container.Resolve<CheckPasswordValid>());
            containerRegistry.RegisterInstance<ICheckEmailValid>(Container.Resolve<CheckEmailValid>());
            containerRegistry.RegisterInstance<IUserAuthorization>(Container.Resolve<UserAuthorization>());
            containerRegistry.RegisterInstance<ICheckNameValid>(Container.Resolve<CheckNameValid>());
            containerRegistry.RegisterInstance<IPermissionService>(Container.Resolve<PermissionService>());
            containerRegistry.RegisterInstance<IPinService>(Container.Resolve<PinService>());            
        }

        public static T Resolve<T>()
        {
            return Current.Container.Resolve<T>();
        }

        protected override void OnInitialized()
        {
            Device.SetFlags(new string[] { "RadioButton_Experimental" });

            if (Resolve<ISettingsService>().IsAuthorized)
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
