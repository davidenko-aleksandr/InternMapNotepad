using Acr.UserDialogs;
using MapNotepad.Models;
using MapNotepad.Resources;
using MapNotepad.Services.NoteService;
using MapNotepad.Sevices.PinServices;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Navigation;
using Prism.Services;
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
        private readonly IPageDialogService _dialogService;

        private int _pinId;
        private NoteForPinModel _noteForPinModel;

        public AddNotePageViewModel(
                                    IPinService pinService,
                                    INoteForPinService noteService,
                                    IPageDialogService dialogService,
                                    INavigationService navigationService) : base(navigationService)
        {
            _pinService = pinService;
            _noteService = noteService;
            _dialogService = dialogService;
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
                else
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

                if (photo != null)
                {
                    ImageSource = photo.Path;
                }
                else
                {
                    ImageSource = "add_photo.png";
                }
            }
        }

        private async Task<bool> SaveNoteForPinAsync()
        {
            bool IsCorrectData;

            if (_noteForPinModel.Id == 0)
            {
                NoteForPinModel note = new NoteForPinModel
                {
                    NoteTitle = NoteLable,
                    NoteDescription = NoteDescription,
                    Image = ImageSource,
                    PinId = _pinId
                };
                IsCorrectData = await SaveOrShowAlert(note);
            }
            else
            {
                _noteForPinModel.NoteTitle = NoteLable;
                _noteForPinModel.NoteDescription = NoteDescription;
                _noteForPinModel.Image = ImageSource;
                IsCorrectData = await SaveOrShowAlert(_noteForPinModel);
            }

            return IsCorrectData;
        }

        private async Task<bool> SaveOrShowAlert(NoteForPinModel note)
        {
            bool IsCorrectData = false;

            if (!string.IsNullOrWhiteSpace(NoteLable))
            {
                await _noteService.AddOrUpdateNoteInDBAsync(note);
                IsCorrectData = true;
            }
            else
            {
                await _dialogService.DisplayAlertAsync(AppResources.AlertError, AppResources.AlertIncorrectNote, AppResources.AlertOk);
            }

            return IsCorrectData;
        }

        private async void OnSaveCommandAsync()
        {
            bool IsCorrectData = await SaveNoteForPinAsync();

            if (IsCorrectData && _pinId != 0)
            {     
                await _pinService.UpdatePinForAddNoteAsync(_pinId);

                await _navigationService.GoBackAsync();
            }
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
