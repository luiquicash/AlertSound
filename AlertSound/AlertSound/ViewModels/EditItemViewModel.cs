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
    public class EditItemViewModel : BaseViewModel
    {
        public EditItemViewModel()
        {
            FromMinimumDate = DateTime.Now;
            ToMinimumDate = DateTime.Now;
            SaveCommand = new Command(OnUpdate, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private string itemId;
        private string text;
        private string description;
        private string soundselected;
        private DateTime from;
        private DateTime to;
        private TimeSpan hour;
        private bool status;
        private bool iseventrepeat;
        private ItemRepeat repeatevent;

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(text);
        }
        public string Id { get; set; }
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
            get => soundselected;
            set => SetProperty(ref soundselected, value);
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
        public bool Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        public bool isEventRepeat
        {
            get => iseventrepeat;
            set => SetProperty(ref iseventrepeat, value);
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

        public DateTime FromMinimumDate { get; }
        public DateTime ToMinimumDate { get; set; }
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
        private async void OnUpdate()
        {
            var item = await DataStore.GetItemAsync(itemId);

            item.Text = Text;
            item.From = From;
            item.To = To;
            item.EventHour = Hour;
            item.Description = Description;
            item.SoundSelected = SoundSelected;
            item.isEventRepeat = isEventRepeat;
            item.Status = Status;
            item.RepeatEvent = RepeatEvent;

            await DataStore.UpdateItemAsync(item);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await DataStore.GetItemAsync(itemId);
                Id = item.Id;
                Text = item.Text;
                Description = item.Description;
                SoundSelected = GetSounds(item.SoundSelected);
                From = item.From;
                To = item.To.Date;
                Hour = item.EventHour;
                isEventRepeat = item.isEventRepeat;
                Status = item.Status;
                RepeatEvent = item.RepeatEvent;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        private string GetSounds(string itemValue)
        {
            string output = string.Empty;
            var response = GetSoundsList().FirstOrDefault(x => x.Value == itemValue);
            if (response != null)
                return response.Value;
            else
                return output;
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
    }
}
