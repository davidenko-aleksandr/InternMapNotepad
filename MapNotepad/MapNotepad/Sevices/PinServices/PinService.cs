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

        public Task<int> AddOrUpdatePinInDBAsync(PinGoogleMapModel pin)
        {
            pin.UserId = _settingsService.CurrentUserID;

            return _repositoryService.SaveOrUpdateItemAsync(pin);
        }

        public async Task DeletePinAsync(int id)
        {
            PinGoogleMapModel pin = await GetByIdAsync(id);

            if (pin != null)
            {
                await _repositoryService.DeleteItemAsync(pin);
            }
        }

        public async Task<IEnumerable<PinGoogleMapModel>> GetPinsFromDBAsync(string filter = null)
        {
            var pinCollection = await _repositoryService.GetItemsAsync<PinGoogleMapModel>();

            var pinsByUresId = pinCollection.Where(p => p.UserId == _settingsService.CurrentUserID);

            if (string.IsNullOrEmpty(filter))
            {     
                return pinsByUresId;
            }
            else
            {
                pinsByUresId = pinsByUresId.Where(p => p.Label.Contains(filter) ||
                                          (p.Description.Contains(filter)) || 
                                          (p.Latitude.ToString().Contains(filter)) || 
                                          (p.Longitude.ToString().Contains(filter)));

                return pinsByUresId;
            }
        }

        public async Task<IEnumerable<PinGoogleMapModel>> GetFavoritePinsFromDBAsync()
        {
            var pinCollection = await _repositoryService.GetItemsAsync<PinGoogleMapModel>();

            var favoritePins = pinCollection.Where(p => p.UserId == _settingsService.CurrentUserID && p.IsFavorite == true);

            return favoritePins;
        }
        public Task<PinGoogleMapModel> GetByIdAsync(int id)
        {
            return _repositoryService.GetItemAsync<PinGoogleMapModel>(p => p.Id == id && p.UserId == _settingsService.CurrentUserID);
        }
    }
}
