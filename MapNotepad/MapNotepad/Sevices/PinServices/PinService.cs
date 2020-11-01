using MapNotepad.Models;
using MapNotepad.Sevices.RepositoryService;
using MapNotepad.Sevices.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

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

            var pins = pinCollection.Where(p => p.UserId == _settingsService.CurrentUserID);

            return pins;
        }

        public Task<PinGoogleMapModel> GetByIdAsync(int id)
        {
            return _repositoryService.GetItemAsync<PinGoogleMapModel>(p => p.Id == id && p.UserId == _settingsService.CurrentUserID);
        }
    }
}
