using MapNotepad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapNotepad.Services.NoteService
{
    public interface INoteForPinService
    {
        Task<int> AddOrUpdateNoteInDBAsync(NoteForPinModel note);

        Task DeleteNoteAsync(int id);

        Task<IEnumerable<NoteForPinModel>> GetNotesFromDBAsync(int pinId);

        Task<NoteForPinModel> GetNoteByIdAsync(int id);

        Task DeleteCollectionNoteAsync(int pinId);
    }
}
