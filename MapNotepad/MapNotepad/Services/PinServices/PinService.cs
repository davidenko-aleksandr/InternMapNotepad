﻿using MapNotepad.Models;
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
            pin.UserId = _settingsService.CurrentUserID;

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
            var pinsByUresId = await _repositoryService.GetItemsAsync<PinGoogleMapModel>(p => p.UserId == _settingsService.CurrentUserID);

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
            var favoriteCollectionPins = await _repositoryService.GetItemsAsync<PinGoogleMapModel>(p => p.UserId == _settingsService.CurrentUserID &&
                                                                                                    p.IsFavorite == true);
            return favoriteCollectionPins;
        }

        public Task<PinGoogleMapModel> GetPinByIdAsync(int id)
        {
            return _repositoryService.GetItemAsync<PinGoogleMapModel>(p => p.Id == id && p.UserId == _settingsService.CurrentUserID);
        }

        public async Task<int> GetPinId(double latitude, double longitude)
        {
            var pin = await _repositoryService.GetItemAsync<PinGoogleMapModel>(p => p.Latitude == latitude && p.Longitude == longitude);

            return (int)pin?.Id; 
        }
    }
}
