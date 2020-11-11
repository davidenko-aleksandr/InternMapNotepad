using MapNotepad.Models;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.Sevices.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapNotepad.Services.NoteService
{
    public class NoteForPinService : INoteForPinService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly ISettingsService _settingsService;

        public NoteForPinService(
                           IRepositoryService repositoryService,
                           ISettingsService settingsService)
        {
            _repositoryService = repositoryService;
            _settingsService = settingsService;
        }

        public Task<int> AddOrUpdateNoteInDBAsync(NoteForPinModel note)
        {
            note.UserEmail = _settingsService.CurrentUserEmail;

            return _repositoryService.SaveOrUpdateItemAsync(note);
        }

        public async Task DeleteNoteAsync(int id)
        {
            NoteForPinModel note = await GetNoteByIdAsync(id);

            if (note != null)
            {
                await _repositoryService.DeleteItemAsync(note);
            }
        }

        public async Task DeleteCollectionNoteAsync(int pinId)
        {
            var noteCollection = await _repositoryService.GetItemsAsync<NoteForPinModel>(n => n.UserEmail == _settingsService.CurrentUserEmail && n.PinId == pinId);
            if (noteCollection.Count() != 0)
            {
                foreach (var note in noteCollection)
                {
                    await DeleteNoteAsync(note.Id);
                }
            }
        }

        public async Task<IEnumerable<NoteForPinModel>> GetNotesFromDBAsync(int pinId)
        {
            var noteCollection = await _repositoryService.GetItemsAsync<NoteForPinModel>(n => n.UserEmail == _settingsService.CurrentUserEmail && n.PinId == pinId);

            return noteCollection;
        }

        public Task<NoteForPinModel> GetNoteByIdAsync(int id)
        {
            return _repositoryService.GetItemAsync<NoteForPinModel>(n => n.Id == id && n.UserEmail == _settingsService.CurrentUserEmail);
        }
    }
}
