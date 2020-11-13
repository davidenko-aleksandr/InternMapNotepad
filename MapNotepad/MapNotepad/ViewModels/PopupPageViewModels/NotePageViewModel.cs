using MapNotepad.Models;
using Prism.Navigation;

namespace MapNotepad.ViewModels.PopupPageViewModels
{
    public class NotePageViewModel : BaseViewModel
    {
        private NoteForPinModel _noteForPinModel;

        public NotePageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        #region -- Public properties --

        private string _noteLable;
        public string NoteLable
        {
            get { return _noteLable; }
            set { SetProperty(ref _noteLable, value); }
        }

        private string _noteDescription;
        public string NoteDescription
        {
            get { return _noteDescription; }
            set { SetProperty(ref _noteDescription, value); }
        }

        private string _imageSource = "add_photo.png";
        public string ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue("note", out _noteForPinModel) && _noteForPinModel != null)
            {
                NoteLable = _noteForPinModel.NoteTitle;
                NoteDescription = _noteForPinModel.NoteDescription;
                ImageSource = _noteForPinModel.Image;
            }
            else
            {
                NoteLable = string.Empty;
                NoteDescription = string.Empty;
                ImageSource = "add_photo.png";
            }
        }
        #endregion
    }
}
