using Acr.UserDialogs;
using MapNotepad.Resources;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Navigation;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace MapNotepad.ViewModels.PopupPageViewModels
{
    public class AddNotePageViewModel : BaseViewModel
    {

        public AddNotePageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        private string _imageSource = "add_photo.png";
        public string ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new Command(OnCancelCommandAsync);

        private ICommand _addPhotoCommand;
        public ICommand AddPhotoCommand => _addPhotoCommand ??= new Command(OnAddPhotoCommand);

        private void OnAddPhotoCommand()
        {
            UserDialogs.Instance.ActionSheet(new ActionSheetConfig()
               .SetCancel(AppResources.AlertCancel)
               .SetTitle(AppResources.AlertAddingPhoto)
               .Add(AppResources.AlertUseCamera, TakeNewPhotoAsync, "camera.png")
               .Add(AppResources.AlertDownlFromGallery, GetPhotoFromGalleryAsync, "gallery.png")
               );
        }

        private async void OnCancelCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }
        private async void TakeNewPhotoAsync()
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    SaveToAlbum = true,
                    Directory = "drawable",
                    Name = $"{DateTime.Now:dd.MM.yyyy_hh.mm.ss}.jpg"
                });

                if (file != null)
                {
                    ImageSource = file.Path;
                }

                if (ImageSource == null)
                {
                    ImageSource = "add_photo.png";
                }
            }
        }

        private async void GetPhotoFromGalleryAsync()
        {
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile photo = await CrossMedia.Current.PickPhotoAsync();
                ImageSource = photo.Path;
                if (ImageSource == null)
                {
                    ImageSource = "add_photo.png";
                }
            }
        }
    }
}
