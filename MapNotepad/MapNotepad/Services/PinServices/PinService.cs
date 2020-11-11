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

        public PinService(
                          IRepositoryService repositoryService, 
                          ISettingsService settingsService)
        {
            _repositoryService = repositoryService;
            _settingsService = settingsService;
        }

        public Task<int> AddOrUpdatePinInDBAsync(PinGoogleMapModel pin)
        {
            pin.UserEmail = _settingsService.CurrentUserEmail;

            return _repositoryService.SaveOrUpdateItemAsync(pin);
        }

        public async Task DeletePinAsync(int id)
        {
            PinGoogleMapModel pin = await GetPinByIdAsync(id);

            if (pin != null)
            {
                await _repositoryService.DeleteItemAsync(pin);
            }
        }

        public async Task<IEnumerable<PinGoogleMapModel>> GetPinsFromDBAsync(string filter = null)
        {
            var pinsByUresId = await _repositoryService.GetItemsAsync<PinGoogleMapModel>(p => p.UserEmail == _settingsService.CurrentUserEmail);

            IEnumerable<PinGoogleMapModel> pinsCollection;

            if (string.IsNullOrEmpty(filter))
            {
                pinsCollection = pinsByUresId;
            }
            else
            {
                pinsCollection = pinsByUresId.Where(p => p.Label.Contains(filter) ||
                                                   (p.Description.Contains(filter)) ||
                                                   (p.Latitude.ToString().Contains(filter)) ||
                                                   (p.Longitude.ToString().Contains(filter)));
            }

            return pinsCollection;
        }

        public async Task<IEnumerable<PinGoogleMapModel>> GetPinsFromDBAsync() //these are favorit pins
        {
            var favoriteCollectionPins = await _repositoryService.GetItemsAsync<PinGoogleMapModel>(p => p.UserEmail == _settingsService.CurrentUserEmail &&
                                                                                                    p.IsFavorite == true);
            return favoriteCollectionPins;
        }

        public Task<PinGoogleMapModel> GetPinByIdAsync(int id)
        {
            return _repositoryService.GetItemAsync<PinGoogleMapModel>(p => p.Id == id && p.UserEmail == _settingsService.CurrentUserEmail);
        }

        public async Task<int> GetPinId(double latitude, double longitude)
        {
            var pin = await _repositoryService.GetItemAsync<PinGoogleMapModel>(p => p.Latitude == latitude && p.Longitude == longitude);

            return (int)pin?.Id; 
        }

        public async Task UpdatePinForAddNoteAsync(int pinId)
        {
            PinGoogleMapModel pinModel;
            pinModel = await GetPinByIdAsync(pinId);

            int countOfNote = pinModel.CountOfNote += 1;

            pinModel.LableForCountOfNote = countOfNote switch
            {
                1 => "1 note",
                2 => "2 note",
                3 => "3 note",
                4 => "4 note",
                5 => "5 note",
                _ => "5+ note",
            };
            await AddOrUpdatePinInDBAsync(pinModel);
        }

        public async Task UpdatePinForRemoveNoteAsync(int pinId)
        {
            PinGoogleMapModel pinModel;
            pinModel = await GetPinByIdAsync(pinId);

            int countOfNote = pinModel.CountOfNote -= 1;

            pinModel.LableForCountOfNote = countOfNote switch
            {
                0 => "",
                1 => "1 note",
                2 => "2 note",
                3 => "3 note",
                4 => "4 note",
                5 => "5 note",
                _ => "5+ note",
            };
            await AddOrUpdatePinInDBAsync(pinModel);
        }
    }
}
