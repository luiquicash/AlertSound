using AlertSound.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AlertSound.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;
        public static List<Item> itemsList { get; set; }
        public MockDataStore()
        {
            if (itemsList is null || itemsList.Count == 0)
                itemsList = new List<Item>();

            items = itemsList;
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            itemsList.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = itemsList.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            itemsList.Remove(oldItem);
            itemsList.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = itemsList.Where((Item arg) => arg.Id == id).FirstOrDefault();
            itemsList.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(itemsList.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(itemsList);
        }

        public async Task<bool> PlayAlarm(string soundName)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream audioStream = assembly.GetManifestResourceStream(soundName);
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(audioStream);
            audio.Play();
            return audio.IsPlaying;
        }
    }
}