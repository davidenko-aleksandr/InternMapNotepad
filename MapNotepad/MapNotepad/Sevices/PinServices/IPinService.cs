using MapNotepad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapNotepad.Sevices.PinServices
{
    public interface IPinService
    {
        Task<int> AddOrUpdatePinInDBAsync(PinGoogleMapModel pin);

        Task DeletePinAsync(int id);

        Task<IEnumerable<PinGoogleMapModel>> GetPinsFromDBAsync(string filter = null);

        Task<PinGoogleMapModel> GetByIdAsync(int id);
        Task<IEnumerable<PinGoogleMapModel>> GetFavoritePinsFromDBAsync();
    }
}
