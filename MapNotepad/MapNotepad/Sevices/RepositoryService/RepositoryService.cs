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
        private readonly SQLiteAsyncConnection database;

        public RepositoryService()
        {
            database = new SQLiteAsyncConnection(Path.Combine(Environment.GetFolderPath(
                                            Environment.SpecialFolder.LocalApplicationData), 
                                            Constants.DATABASE_NAME));
        }

        public async Task<int> DeleteItemAsync<T>(T item) where T : IBaseModel, new()
        {
            await database.CreateTableAsync<T>();

            var delete = await database.DeleteAsync(item);

            return delete;
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>() where T : IBaseModel, new()
        {
            await database.CreateTableAsync<T>();

            var collection = await database.Table<T>().ToListAsync();

            return collection;
        }

        public async Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel, new()
        {
            await database.CreateTableAsync<T>();

            var item = await database.FindAsync(predicate);

            return item;
        }

        public async Task<int> SaveOrUpdateItemAsync<T>(T item) where T : IBaseModel, new()
        {
            await database.CreateTableAsync<T>();

            int itemId;

            itemId = item.Id != 0 ? await database.UpdateAsync(item) : _ = await database.InsertAsync(item);

            return itemId;
        }
    }
}
