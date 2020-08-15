using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prism.Services;
using TestAppKontur.Models.Data;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TestAppKontur.Models.Dao
{
    public class Dao
    {
        private static ContactDbContext _db;

        private static Dao instance;
        public static List<Contact> ContactList { get; set; } = new List<Contact>();
        private Dao()
        {
        }
        public static Dao GetInstance(string dbPath)
        {
            if (instance == null)
            {
                instance = new Dao();
                _db = new ContactDbContext(dbPath);
            }
            return instance;
        }
        public static async Task<bool> UpdateContacts()
        {

            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                return await Dao.GetContactsFromWeb(false);
            }
            else
            {
                await Dao.GetContactsFromDB();
                return true;
            }
        }
        public static async Task GetContactsFromDB()
        {
            ContactList = await _db.GetContactsAsync();
        }
        public static List<Contact> GetSearchResults(string query)
        {
            var results = new List<Contact>();
            foreach (var strQuery in query.Split(' '))
            {
                results.AddRange(ContactList.Where(x => (x.Name.Contains(strQuery) || 
                    string.Join("", x.Phone.Where(c => char.IsDigit(c))).Contains(strQuery))
                        &&!results.Any(y=>y==x)));
            }
            return results;
        }
        public async static Task<bool> GetContactsFromWeb(bool isUpdated)
        {
            try
            {
                //Если необходимо ошибку вызвать
                //throw new HttpListenerException();
                string json;
                using (WebClient client = new WebClient())
                {
                    json = client.DownloadString($"https://raw.githubusercontent.com/Newbilius/" +
                        $"ElbaMobileXamarinDeveloperTest/master/json/generated-01.json");
                }

                // using newtonsoft json.net - use http://json2csharp.com/ to verfiy
                // that your C# model class actually matches your json
                var data = JsonConvert.DeserializeObject<List<Contact>>(json);
                _db.ClearDbAsync();
                foreach (Contact contact in data)
                {
                    var eduId = Guid.NewGuid().ToString();
                    contact.EducationPeriodId = eduId;
                    contact.EducationPeriod = contact.EducationPeriod;
                    contact.EducationPeriod.Id = eduId;
                }
                await _db.SaveAllItemsAsync(data.Select(x => x.EducationPeriod), isUpdated);
                await _db.SaveAllItemsAsync(data, isUpdated);
                ContactList = data;
                return true;
            }
            catch
            {
                _db.ClearDbAsync();
                foreach (Contact contact in ContactList)
                {
                    var eduId = Guid.NewGuid().ToString();
                    contact.EducationPeriodId = eduId;
                    contact.EducationPeriod = contact.EducationPeriod;
                    contact.EducationPeriod.Id = eduId;
                }
                await _db.SaveAllItemsAsync(ContactList.Select(x => x.EducationPeriod), isUpdated);
                await _db.SaveAllItemsAsync(ContactList, isUpdated);
                return false;
            }
        }
    }
}
