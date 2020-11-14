using MapNotepad.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MapNotepad.Sevices.RepositoryService
{
    public class RepositoryService : IRepositoryService
    {       
        private readonly SQLiteAsyncConnection _database;

        public RepositoryService()
        {
            _database = new SQLiteAsyncConnection(
                                                  Path.Combine(Environment.GetFolderPath(
                                                  Environment.SpecialFolder.LocalApplicationData), 
                                                  Constants.DATABASE_NAME));
        }

        public async Task<int> DeleteItemAsync<T>(T item) where T : IBaseModel, new()
        {
            await _database.CreateTableAsync<T>();

            var delete = await _database.DeleteAsync(item);

            return delete;
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel, new()
        {
            await _database.CreateTableAsync<T>();

            var table = _database.Table<T>();

            List<T> collection;

            if (predicate == null)
            {
                collection = await table.ToListAsync();
            }
            else
            {
                collection = await table.Where(predicate).ToListAsync();
            }
            return collection;
        }

        public async Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel, new()
        {
            await _database.CreateTableAsync<T>();

            var item = await _database.FindAsync(predicate);

            return item;
        }

        public async Task<int> SaveOrUpdateItemAsync<T>(T item) where T : IBaseModel, new()
        {
            await _database.CreateTableAsync<T>();

            int itemId = item.Id != 0 ? await _database.UpdateAsync(item) : await _database.InsertAsync(item);

            return itemId;
        }
    }
}
