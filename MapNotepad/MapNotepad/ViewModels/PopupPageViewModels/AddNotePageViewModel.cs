using Prism.Navigation;
using Rg.Plugins.Popup.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace MapNotepad.ViewModels.PopupPageViewModels
{
    public class AddNotePageViewModel : BaseViewModel
    {

        public AddNotePageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new Command(CancelWriteNote);

        private async void CancelWriteNote()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
