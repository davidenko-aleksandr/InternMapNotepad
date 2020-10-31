using MapNotepad.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MapNotepad.Sevices.RepositoryService
{
    public interface IRepositoryService
    {
        Task<IEnumerable<T>> GetItemsAsync<T>() where T : IBaseModel, new();

        Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel, new();

        Task<int> SaveOrUpdateItemAsync<T>(T item) where T : IBaseModel, new();

        Task<int> DeleteItemAsync<T>(T item) where T : IBaseModel, new();
    }
}