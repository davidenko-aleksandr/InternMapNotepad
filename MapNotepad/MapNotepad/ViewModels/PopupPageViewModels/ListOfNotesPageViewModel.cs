using MapNotepad.Models;
using MapNotepad.Services.NoteService;
using MapNotepad.Sevices.PinServices;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MapNotepad.ViewModels.PopupPageViewModels
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

        private ObservableCollection<NoteForPinModel> _collectionNotes;
        public ObservableCollection<NoteForPinModel> CollectionNotes
        {
            get { return _collectionNotes; }
            set { SetProperty(ref _collectionNotes, value); }
        }
        public async Task InitCollectionNotesAsync()
        {
            var collection = await _noteService.GetNotesFromDBAsync(_pinId);

            CollectionNotes = new ObservableCollection<NoteForPinModel>(collection);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.SELECT_PIN, out int pin) && pin != 0)
            {
                _pinId = pin;
            }
            await InitCollectionNotesAsync();
        }
    }
}
