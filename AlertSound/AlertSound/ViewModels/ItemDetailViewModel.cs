using AlertSound.Models;
using AlertSound.Models.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace AlertSound.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string itemId;
        private string text;
        private string description;
        private string soundseleted;
        private bool isrepeatevent;
        private DateTime from;
        private DateTime to;
        private TimeSpan hour;
        public string status;
        public string iseventrepeat;
        public ItemRepeat repeatevent;

        public string Id { get; set; }
        public string StatusStr
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        public string isEventRepeatStr
        {
            get => iseventrepeat;
            set => SetProperty(ref iseventrepeat, value);
        }
        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }
        public bool isEventRepeat
        {
            get => isrepeatevent;
            set => SetProperty(ref isrepeatevent, value);
        }
        public string SoundSelected
        {
            get => soundseleted;
            set => SetProperty(ref soundseleted, value);
        }
        public DateTime From
        {
            get => from;
            set => SetProperty(ref from, value);
        }
        public DateTime To
        {
            get => to;
            set => SetProperty(ref to, value);
        }
        public TimeSpan Hour
        {
            get => hour;
            set => SetProperty(ref hour, value);
        }
        public ItemRepeat RepeatEvent
        {
            get => repeatevent;
            set => SetProperty(ref repeatevent, value);
        }
        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                Id = item.Id;
                Text = item.Text;
                Description = item.Description;
                SoundSelected = GetSoundsByValue(item.SoundSelected);
                From = item.From;
                To = item.To.Date;
                Hour = item.EventHour;
                RepeatEvent = item.RepeatEvent;
                isEventRepeat = item.isEventRepeat;
                isEventRepeatStr = item.isEventRepeat ? "Si" : "No";
                StatusStr = item.Status ? "Activo" : "InActivo";
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
        private List<ItemQuantityType> GetSoundsList()
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
            var outputList = new List<ItemQuantityType>();
            foreach (var item in db)
            {
                var newitem = new ItemQuantityType()
                {
                    Value = root + item,
                    Name = "sound" + sound
                };
                outputList.Add(newitem);
                sound++;
            }
            return outputList;
        }
        private string GetSoundsByValue(string itemValue)
        {
            string output = string.Empty;
            var response = GetSoundsList().FirstOrDefault(x => x.Value == itemValue);
            if (response != null)
                return response.Name;
            else
                return output;
        }
    }
}
