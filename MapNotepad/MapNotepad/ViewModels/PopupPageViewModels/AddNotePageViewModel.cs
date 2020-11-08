using Acr.UserDialogs;
using MapNotepad.Models;
using MapNotepad.Resources;
using MapNotepad.Services.NoteService;
using MapNotepad.Sevices.PinServices;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace MapNotepad.ViewModels.PopupPageViewModels
{
    public class AddNotePageViewModel : BaseViewModel
    {
        private readonly IPinService _pinService;
        private readonly INoteForPinService _noteService;

        private int _pinId;

        public AddNotePageViewModel(
                                    IPinService pinService,
                                    INoteForPinService noteService,
                                    INavigationService navigationService) : base(navigationService)
        {
            _pinService = pinService;
            _noteService = noteService;
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

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new Command(OnCancelCommandAsync);

        private ICommand _addPhotoCommand;
        public ICommand AddPhotoCommand => _addPhotoCommand ??= new Command(OnAddPhotoCommand);

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new Command(OnSaveCommandAsync);

        #endregion

        private async void OnSaveCommandAsync()
        {
            await SaveNoteForPinAsync();

            await UpdatePinForAddNoteAsync();

            await _navigationService.GoBackAsync();
        }

        private void OnAddPhotoCommand()
        {
            UserDialogs.Instance.ActionSheet(
                                 new ActionSheetConfig()
                                .SetCancel(AppResources.AlertCancel)
                                .SetTitle(AppResources.AlertAddingPhoto)
                                .Add(AppResources.AlertUseCamera, TakeNewPhotoAsync, "camera.png")
                                .Add(AppResources.AlertDownlFromGallery, GetPhotoFromGalleryAsync, "gallery.png"));
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

        private async Task SaveNoteForPinAsync()
        {
            NoteForPinModel note = new NoteForPinModel
            {
                NoteTitle = NoteLable,
                NoteDescription = NoteDescription,
                Image = ImageSource,
                PinId = _pinId
            };

            await _noteService.AddOrUpdateNoteInDBAsync(note);
        }

        private async Task UpdatePinForAddNoteAsync()
        {
            PinGoogleMapModel pinModel;
            pinModel = await _pinService.GetPinByIdAsync(_pinId);                      

            int countOfNote = pinModel.CountOfNote += 1;

            pinModel.LableForCountOfNote = countOfNote switch
            {
                1 => "1 note",
                2 => "2 note",
                3 => "3 note",
                4 => "4 note",
                5 => "5 note",
                _ => "5+ note",
            };
            await _pinService.AddOrUpdatePinInDBAsync(pinModel);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.SELECT_PIN, out int pin) && pin != 0)
            {
                _pinId = pin;
            }
        }
    }
}
