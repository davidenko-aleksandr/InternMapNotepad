using Acr.UserDialogs;
using MapNotepad.Models;
using MapNotepad.Resources;
using MapNotepad.Services.NoteService;
using MapNotepad.Sevices.PinServices;
using MapNotepad.Views.PopupPageViews;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MapNotepad.ViewModels
{
    public class ListOfNotesPageViewModel : BaseViewModel
    {
        private readonly IPinService _pinService;
        private readonly INoteForPinService _noteService;

        private int _pinId;

        public ListOfNotesPageViewModel(
                                        IPinService pinService,
                                        INoteForPinService noteService,
                                        INavigationService navigationService) : base(navigationService)
        {
            _pinService = pinService;
            _noteService = noteService;
        }

        #region -- Public properties --

        private ObservableCollection<NoteForPinModel> _collectionNotes;
        public ObservableCollection<NoteForPinModel> CollectionNotes
        {
            get { return _collectionNotes; }
            set { SetProperty(ref _collectionNotes, value); }
        }

        private ICommand _editPinCommand;
        public ICommand EditPinCommand => _editPinCommand ??= new Command<NoteForPinModel>(OnEditPinSelectedCommandAsync);

        private ICommand _deletePinCommand;
        public ICommand DeletePinCommand => _deletePinCommand ??= new Command<NoteForPinModel>(OnDeletePinSelectedCommandAsync);

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new Command(OnComeBackCommandAsync);

        private ICommand _openAddNoteViewPageCommand;
        public ICommand OpenAddNoteViewPageCommand => _openAddNoteViewPageCommand ??= new Command(OnOpenAddNoteViewPageCommandAsync);

        private ICommand _selectedNoteCommand;
        public ICommand SelectedNoteCommand => _selectedNoteCommand ??= new Command<object>(OnNoteSelectedCommandAsync);

        #endregion

        #region -- Private helpers --
        private async void OnNoteSelectedCommandAsync(object obj)
        {
            NoteForPinModel note = new NoteForPinModel();
            if (obj is NoteForPinModel selectedNote)
            {
                note = selectedNote;
                NavigationParameters parameters = new NavigationParameters { { "note", note } };
                await _navigationService.NavigateAsync($"{nameof(NotePageView)}", parameters);
            }         
        }

        private async void OnComeBackCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private async void OnOpenAddNoteViewPageCommandAsync()
        {
            NavigationParameters parameters = new NavigationParameters { { Constants.SELECTED_PIN, _pinId } };

            await _navigationService.NavigateAsync($"{nameof(AddNotePageView)}", parameters);
        }

        private async void OnEditPinSelectedCommandAsync(NoteForPinModel selectedNote)
        {
            NavigationParameters parameters = new NavigationParameters { { Constants.SELECTED_NOTE, selectedNote } };

            await _navigationService.NavigateAsync($"{nameof(AddNotePageView)}", parameters);
        }

        private async void OnDeletePinSelectedCommandAsync(NoteForPinModel selectedNote)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Message = AppResources.AlertConfirmDelete,
                OkText = AppResources.AlertDelete,
                CancelText = AppResources.AlertCancel
            });

            if (result == true)
            {
                await _noteService.DeleteNoteAsync(selectedNote.Id);
                await InitCollectionNotesAsync();
                await _pinService.UpdatePinForRemoveNoteAsync(_pinId);
            }
        }

        private async Task InitCollectionNotesAsync()
        {
            var collection = await _noteService.GetNotesFromDBAsync(_pinId);

            CollectionNotes = new ObservableCollection<NoteForPinModel>(collection);
        }

        #endregion

        #region -- Overrides --
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.SELECTED_PIN, out int pin) && pin != 0)
            {
                _pinId = pin;
            }

            await InitCollectionNotesAsync();
        }
        #endregion
    }
}
