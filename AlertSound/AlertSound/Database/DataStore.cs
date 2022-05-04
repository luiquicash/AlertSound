using AlertSound.Constants;
using AlertSound.Models;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AlertSound.Database
{
    public class DataStore
    {
        public readonly SQLiteAsyncConnection _database;
        public DataStore(string StringConnection)
        {
            _database = new SQLiteAsyncConnection(StringConnection);
            _database.CreateTableAsync<Events>();
        }

        public async Task<bool> AddEventAsync(Events item)
        {
            var added = await _database.InsertAsync(item);
            var result = added > 0;
            return await Task.FromResult(result);
        }

        public async Task<bool> UpdateEventAsync(Events item)
        {
            var updated = await _database.UpdateAsync(item);
            var result = updated > 0;
            return await Task.FromResult(result);
        }

        public async Task<bool> DeleteEventAsync(string id)
        {
            var listEvents = await _database.Table<Events>().ToListAsync();
            var oldItem = listEvents.FirstOrDefault(arg => arg.Id == id);
            var deleted = await _database.DeleteAsync(oldItem);
            var result = deleted > 0;
            return await Task.FromResult(result);
        }

        public async Task<Events> GetEventAsync(string id)
        {
            var listEvents = await _database.Table<Events>().ToListAsync();
            return await Task.FromResult(listEvents.FirstOrDefault(arg => arg.Id == id));
        }

        public async Task<IEnumerable<Events>> GetEventListsAsync(bool forceRefresh = false)
        {
            var listEvents = await _database.Table<Events>().ToListAsync();
            return await Task.FromResult(listEvents);
        }

        public List<EventsQuantityType> GetQuantityTypes()
        {
            var db = new List<string>()
            {
                QuantityTypeConstants.Days,
                QuantityTypeConstants.Week,
                QuantityTypeConstants.Month,
                QuantityTypeConstants.Years
            };

            var outputList = new List<EventsQuantityType>();
            var id = 0;
            foreach (var item in db)
            {
                id++;
                var newitem = new EventsQuantityType()
                {
                    Value = id.ToString(),
                    Name = item
                };
                outputList.Add(newitem);
            }

            return outputList;
        }

        public List<EventsQuantityType> GetSoundsList()
        {
            var db = new List<string>()
            {
                SoundsNameConstants.sound1,
                SoundsNameConstants.sound2,
                SoundsNameConstants.sound3,
                SoundsNameConstants.sound4,
                SoundsNameConstants.sound5,
                SoundsNameConstants.sound6,
                SoundsNameConstants.sound7
            };

            var sound = 1;
            var root = "AlertSound.Sounds.";
            var outputList = new List<EventsQuantityType>();
            foreach (var item in db)
            {
                var newitem = new EventsQuantityType()
                {
                    Value = root + item,
                    Name = "sound" + sound
                };
                outputList.Add(newitem);
                sound++;
            }
            return outputList;
        }

        public void PlayAlarm(string soundValue)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream audioStream = assembly.GetManifestResourceStream(soundValue);
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(audioStream);
            audio.Play();
        }

        public void StopAlarm(string soundValue)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream audioStream = assembly.GetManifestResourceStream(soundValue);
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(audioStream);
            audio.Stop();
        }
    }
}