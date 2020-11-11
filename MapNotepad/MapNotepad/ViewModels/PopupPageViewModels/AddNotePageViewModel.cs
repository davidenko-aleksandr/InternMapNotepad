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

namespace MapNotepad.ViewModels.PopupPageViewModels
{
    public class AddNotePageViewModel : BaseViewModel
    {
        private readonly IPinService _pinService;
        private readonly INoteForPinService _noteService;

        private int _pinId;
        private NoteForPinModel _noteForPinModel;

        public AddNotePageViewModel(
                                    IPinService pinService,
                                    INoteForPinService noteService,
                                    INavigationService navigationService) : base(navigationService)
        {
            _pinService = pinService;
            _noteService = noteService;
            _noteForPinModel = new NoteForPinModel();
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
                MediaFile photo = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Small 
                });
               
                ImageSource = photo.Path;
                if (ImageSource == null)
                {
                    ImageSource = "add_photo.png";
                }
            }
        }

        private async Task SaveNoteForPinAsync()
        {
            if (_noteForPinModel.Id == 0)
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
            else
            {
                _noteForPinModel.NoteTitle = NoteLable;
                _noteForPinModel.NoteDescription = NoteDescription;
                _noteForPinModel.Image = ImageSource;
                await _noteService.AddOrUpdateNoteInDBAsync(_noteForPinModel);
            }
        }
        private async void OnSaveCommandAsync()
        {
            await SaveNoteForPinAsync();

            if (_pinId != 0)
            {
                await _pinService.UpdatePinForAddNoteAsync(_pinId);
            }

            await _navigationService.GoBackAsync();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.SELECTED_PIN, out int pin) && pin != 0)
            {
                _pinId = pin;
            }

            if (parameters.TryGetValue(Constants.SELECTED_NOTE, out NoteForPinModel note) && note != null)
            {
                _noteForPinModel = note;

                NoteLable = _noteForPinModel.NoteTitle;
                NoteDescription = _noteForPinModel.NoteDescription;
                ImageSource = _noteForPinModel.Image;
            }
        }
    }
}
