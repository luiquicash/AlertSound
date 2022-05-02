using AlertSound.Models;
using System;
using System.Diagnostics;
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
                SoundSelected = item.SoundSelected;
                From = item.From;
                To = item.To.Date;
                Hour = item.EventHour;
                RepeatEvent = item.RepeatEvent;
                isEventRepeatStr = item.isEventRepeat ? "Si" : "No";
                StatusStr = item.Status ? "Activo" : "InActivo";
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
