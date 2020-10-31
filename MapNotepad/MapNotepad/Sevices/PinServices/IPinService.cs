using MapNotepad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapNotepad.Sevices.PinServices
{
    public interface IPinService
    {
        Task<int> AddPinToDBAsync(PinGoogleMap pin);

        Task DeletePinAsync(int id);

        Task<IEnumerable<PinGoogleMap>> GetPinsFromDBAsync(string filter = null);

        Task<PinGoogleMap> GetById(int id);
    }
}
