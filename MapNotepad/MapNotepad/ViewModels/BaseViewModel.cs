using Prism.Mvvm;
using Prism.Navigation;

namespace MapNotepad.ViewModels
{
    public class BaseViewModel : BindableBase, INavigatedAware, IConfirmNavigation
    {
        public readonly INavigationService _navigationService;

        public BaseViewModel(INavigationService navigationService) 
        {
            _navigationService = navigationService;
        }

        public virtual bool CanNavigate(INavigationParameters parameters)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
            
        }
    }
}
