using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace TestAppKontur.Models.Data
{
    public class ContactDbContext
    {
        readonly SQLiteAsyncConnection database;
        public ContactDbContext(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            //database.DropTableAsync<Educationperiod>().Wait();
            //database.DropTableAsync<Contact>().Wait();
            database.CreateTableAsync<Educationperiod>().Wait();
            database.CreateTableAsync<Contact>().Wait();
        }
        public async void ClearDbAsync()
        {
            await database.DeleteAllAsync<Educationperiod>();
            await database.DeleteAllAsync<Contact>();
        }
        public async Task<List<Contact>> GetContactsAsync()
        {
            var eduPeriods = await database.Table<Educationperiod>().ToListAsync();
            var dictEduPeriods = eduPeriods.ToDictionary((x) => x.Id);
            var contacts = await database.Table<Contact>().ToListAsync();
            contacts.ForEach(x =>
            {
                x.EducationPeriod = dictEduPeriods[x.EducationPeriodId];
            });
            return contacts;
        }
        public Task<int> SaveItemAsync<T>(T item, bool IsUpdated)
        {
            if (IsUpdated)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }
        public Task<int> SaveAllItemsAsync<T>(IEnumerable<T> items, bool IsUpdated)
        {
            if (IsUpdated)
            {
                return database.UpdateAllAsync(items);
            }
            else
            {
                return database.InsertAllAsync(items);
            }
        }
    }
}
