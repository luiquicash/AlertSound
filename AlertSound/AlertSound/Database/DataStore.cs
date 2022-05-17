using AlertSound.Constants;
using AlertSound.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace AlertSound.Database
{
    public class DataStore
    {
        public readonly SQLiteAsyncConnection _database;
        private Timer aTimer;
        public DataStore(string StringConnection)
        {
            _database = new SQLiteAsyncConnection(StringConnection);
            _database.CreateTableAsync<Events>();
        }

        public async Task<bool> AddEventAsync(Events item)
        {
            if ((item.To != null && DateTime.Now.Date <= item.To.Value.Date) || item.From.Date >= DateTime.Now.Date)
            {
                if (ViewModelList.staticAlarmList is null || !ViewModelList.staticAlarmList.Any())
                    ViewModelList.staticAlarmList = new List<Events>();

                ViewModelList.staticAlarmList.Add(item);
            }

            var added = await _database.InsertAsync(item);
            var result = added > 0;
            return await Task.FromResult(result);
        }

        public async Task<bool> UpdateEventAsync(Events item)
        {
            if (ViewModelList.staticAlarmList != null && ViewModelList.staticAlarmList.Any())
            {
                var oldItem = ViewModelList.staticAlarmList.FirstOrDefault(x => x.Id == item.Id);
                ViewModelList.staticAlarmList.Remove(oldItem);
                ViewModelList.staticAlarmList.Add(item);
            }

            var updated = await _database.UpdateAsync(item);
            var result = updated > 0;
            return await Task.FromResult(result);
        }

        public async Task<bool> DeleteEventAsync(string id)
        {
            var events = await _database.Table<Events>().FirstOrDefaultAsync(arg => arg.Id == id);
            var oldItem = events;
            var deleted = await _database.DeleteAsync(oldItem);
            var result = deleted > 0;

            return await Task.FromResult(result);
        }

        public async Task<Events> GetEventAsync(string id)
        {
            var evenT = new Events();
            var events = await _database.Table<Events>().FirstOrDefaultAsync(arg => arg.Id == id);
            if (events != null && !string.IsNullOrWhiteSpace(events.Id))
                evenT = events;

            return await Task.FromResult(evenT);
        }

        public async Task<List<Events>> GetEventListsAsync(bool forceRefresh = false)
        {
            var listEvents = Enumerable.Empty<Events>().ToList();
            var list = await _database.Table<Events>().ToListAsync();
            listEvents = list != null && list.Count > 0 ? list : new List<Events>();

            return await Task.FromResult(listEvents);
        }

        public async Task<List<Events>> GetEventListsByDayAsync(DateTime day)
        {
            var listEvents = Enumerable.Empty<Events>().ToList();
            var list = await GetEventListsAsync();
            var hasItemInList = list != null && list.Count > 0;
            if (hasItemInList)
            {
                foreach (var item in list)
                {
                    if ((item.To != null && day <= item.To.Value.Date) || item.From.Date >= day)
                    {
                        listEvents.Add(item);
                    }
                }
            }

            listEvents = hasItemInList ? list : new List<Events>();

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

        public void PlayAlarm(Events item, bool enabled, bool autoReset)
        {
            if (aTimer is null)
                aTimer = new Timer();

            if (!string.IsNullOrWhiteSpace(item.SoundSelected) && enabled && autoReset)
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                Stream audioStream = assembly.GetManifestResourceStream(item.SoundSelected);
                var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                audio.Load(audioStream);

                aTimer.Elapsed += delegate
                {
                    audio.Play();
                };

                var secondStr = audio.Duration > 0 && audio.Duration.ToString().Contains(",") 
                              ? audio.Duration.ToString().Split(',')[0] 
                              : audio.Duration.ToString();

                var second = Convert.ToDouble(secondStr);
                var miliseconde = second * 1000;

                aTimer.Interval = miliseconde;
                aTimer.Enabled = enabled;
                aTimer.AutoReset = autoReset;
            }
        }

        public void StopAlarm(string soundValue)
        {
            if (!string.IsNullOrWhiteSpace(soundValue))
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                Stream audioStream = assembly.GetManifestResourceStream(soundValue);
                var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                audio.Load(audioStream);
                if (aTimer != null)
                {
                    aTimer.Enabled = false;
                    aTimer.Stop();
                }
                audio.Stop();
            }
        }
    }
}