using AlertSound.Models;
using AlertSound.Models.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace AlertSound.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class EditItemViewModel : BaseViewModel
    {
        private string description;
        private string text;
        private string soundselected;

        private DateTime from;
        private DateTime to;
        private TimeSpan eventHour;

        private bool status;
        private bool iseventrepeat;

        private int quantity;
        private string quantityType;

        private bool isplaybuttonvisible;
        private bool isstopbuttonvisible;

        private int indexsound;
        private int indexquantity;

        private string itemId;
        private ItemRepeat repeatevent;

        public EditItemViewModel()
        {
            isPlayButtonVisible = true;
            isStopButtonVisible = false;
            eventHour = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
            FromMinimumDate = DateTime.Now;
            ToMinimumDate = DateTime.Now;
            SaveCommand = new Command(OnUpdate, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PlayCommand = new Command(PlaySound);
            StopCommand = new Command(StopSound);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
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
        public TimeSpan EventHour
        {
            get => eventHour;
            set => SetProperty(ref eventHour, value);
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
        public int Quantity
        {
            get => quantity;
            set => SetProperty(ref quantity, value);
        }
        public string QuantityType
        {
            get => quantityType;
            set => SetProperty(ref quantityType, value);
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
        public int IndexSound
        {
            get => indexsound;
            set => SetProperty(ref indexsound, value);
        }
        public int IndexQuantity
        {
            get => indexquantity;
            set => SetProperty(ref indexquantity, value);
        }
        public bool isPlayButtonVisible
        {
            get => isplaybuttonvisible;
            set => SetProperty(ref isplaybuttonvisible, value);
        }
        public bool isStopButtonVisible
        {
            get => isstopbuttonvisible;
            set => SetProperty(ref isstopbuttonvisible, value);
        }
        public ItemRepeat RepeatEvent
        {
            get => repeatevent;
            set => SetProperty(ref repeatevent, value);
        }
        public DateTime FromMinimumDate { get; }
        public DateTime ToMinimumDate { get; set; }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command PlayCommand { get; }
        public Command StopCommand { get; }

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
        private List<ItemQuantityType> GetQuantityTypes()
        {
            var db = new List<string>()
            {
                QuantityTypeConstants.Days,
                QuantityTypeConstants.Week,
                QuantityTypeConstants.Month,
                QuantityTypeConstants.Years
            };

            var outputList = new List<ItemQuantityType>();
            var id = 0;
            foreach (var item in db)
            {
                id++;
                var newitem = new ItemQuantityType()
                {
                    Value = id.ToString(),
                    Name = item
                };
                outputList.Add(newitem);
            }

            return outputList;
        }
        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(text);
        }
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
            item.EventHour = EventHour;
            item.Description = Description;
            item.SoundSelected = GetSoundsByName(SoundSelected);
            item.isEventRepeat = isEventRepeat;
            item.Status = Status;

            if (isEventRepeat)
            {
                item.RepeatEvent = new ItemRepeat()
                {
                    Quantity = Quantity,
                    QuantityType = QuantityType
                };
            }

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
                SoundSelected = GetSoundsByValue(item.SoundSelected);
                From = item.From;
                To = item.To.Date;
                EventHour = item.EventHour;
                RepeatEvent = item.RepeatEvent;
                isEventRepeat = item.isEventRepeat;
                Status = item.Status;
                Quantity = item.RepeatEvent.Quantity;
                QuantityType = item.RepeatEvent.QuantityType;
                IndexSound = GetIndexSoundByValue(item.SoundSelected);
                IndexQuantity = GetIndexQuantityByValue(item.RepeatEvent.QuantityType);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
        private void PlaySound()
        {
            if (string.IsNullOrWhiteSpace(SoundSelected))
                return;

            var soundValue = GetSoundsByName(SoundSelected);
            PlayAlarm(soundValue);
            isPlayButtonVisible = false;
            isStopButtonVisible = true;
        }
        private void StopSound()
        {
            if (string.IsNullOrWhiteSpace(SoundSelected))
                return;

            var soundValue = GetSoundsByName(SoundSelected);
            StopAlarm(soundValue);
            isPlayButtonVisible = true;
            isStopButtonVisible = false;
        }
        private void PlayAlarm(string soundValue)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream audioStream = assembly.GetManifestResourceStream(soundValue);
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(audioStream);
            audio.Play();
        }
        private void StopAlarm(string soundValue)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream audioStream = assembly.GetManifestResourceStream(soundValue);
            var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Load(audioStream);
            audio.Stop();
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
        private string GetSoundsByName(string itemName)
        {
            string output = string.Empty;
            var response = GetSoundsList().FirstOrDefault(x => x.Name == itemName);
            if (response != null)
                return response.Value;
            else
                return output;
        }
        private int GetIndexSoundByValue(string itemValue)
        {
            int output = 0;
            var index = GetSoundsList().Select(x => x.Value).ToList().FindIndex(a => a.Contains(itemValue));
            if (index > 0)
                return index;
            else
                return output;
        }
        private int GetIndexQuantityByValue(string itemName)
        {
            int output = 0;
            var index = GetQuantityTypes().Select(x => x.Name).ToList().FindIndex(a => a.Contains(itemName));
            if (index > 0)
                return index;
            else
                return output;
        }
    }
}
