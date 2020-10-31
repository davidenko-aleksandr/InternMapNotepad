using MapNotepad.Models;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.Sevices.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapNotepad.Sevices.PinServices
{
    public class PinService : IPinService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly ISettingsService _settingsService;

        public PinService(IRepositoryService repositoryService, ISettingsService settingsService)
        {
            _repositoryService = repositoryService;
            _settingsService = settingsService;
        }

        public Task<int> AddPinToDBAsync(PinGoogleMap pin)
        {
            pin.User_Id = _settingsService.CurrentUserID;
            pin.Image = "no_favorite.png";
            return _repositoryService.SaveOrUpdateItemAsync(pin);
        }

        public async Task DeletePinAsync(int id)
        {
            PinGoogleMap pin = await GetById(id);
            if (pin != null)
            {
                await _repositoryService.DeleteItemAsync(pin);
            }
        }

        public async Task<IEnumerable<PinGoogleMap>> GetPinsFromDBAsync(string filter = null)
        {
            var pinCollection = await _repositoryService.GetItemsAsync<PinGoogleMap>();
            var pins = pinCollection.ToList().Where(p => p.User_Id == _settingsService.CurrentUserID);
            return pins;
        }

        public Task<PinGoogleMap> GetById(int id)
        {
            return _repositoryService.GetItemAsync<PinGoogleMap>(p => p.Id == id && p.User_Id == _settingsService.CurrentUserID);
        }
    }
}
