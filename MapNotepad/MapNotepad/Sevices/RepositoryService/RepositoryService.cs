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
        public const string DATABASE_NAME = "user.db";

        private readonly SQLiteAsyncConnection database;

        public RepositoryService()
        {
            database = new SQLiteAsyncConnection(Path.Combine(Environment.GetFolderPath(
                                            Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME));
        }
        public async Task<int> DeleteItemAsync<T>(T item) where T : IBaseModel, new()
        {
            await database.CreateTableAsync<T>();
            return await database.DeleteAsync(item);
        }

        public async Task<IEnumerable<T>> GetItemsAsync<T>() where T : IBaseModel, new()
        {
            await database.CreateTableAsync<T>();


 
            return await database.Table<T>().ToListAsync();
        }

        public async Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel, new()
        {
            await database.CreateTableAsync<T>();
            return await database.FindAsync(predicate);
        }

        public async Task<int> SaveOrUpdateItemAsync<T>(T item) where T : IBaseModel, new()
        {
            await database.CreateTableAsync<T>();

            int result;
            
            if (item.Id != 0)
            {
                result = await database.UpdateAsync(item);
            }
            else
            {
                result = await database.InsertAsync(item);
            }

            return result;
        }
    }
}
