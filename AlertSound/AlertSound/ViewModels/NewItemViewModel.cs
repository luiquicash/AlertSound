using AlertSound.Models;
using AlertSound.Models.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace AlertSound.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string description;
        private string text;
        private string soundselected;

        private DateTime from;
        private DateTime to;
        private TimeSpan eventHour;

        private bool status;
        private bool iseventrepeat;

        private int indexsound;
        private int indexquantity;

        private bool isplaybuttonvisible;
        private bool isstopbuttonvisible;

        private int quantity;
        private string quantityType;

        public NewItemViewModel()
        {
            indexsound = 0;
            isPlayButtonVisible = true;
            isStopButtonVisible = false;
            eventHour = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
            FromMinimumDate = DateTime.Now;
            ToMinimumDate = DateTime.Now;
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PlayCommand = new Command(PlaySound);
            StopCommand = new Command(StopSound);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
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
        public bool isEventRepeat
        {
            get => iseventrepeat;
            set => SetProperty(ref iseventrepeat, value);
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
        public DateTime FromMinimumDate { get; }
        public DateTime ToMinimumDate { get; set; }
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command PlayCommand { get; }
        public Command StopCommand { get; }

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
        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(text);
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

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
        private async void OnSave()
        {
            Item newItem = new Item()
            {
                Id = Guid.NewGuid().ToString().Replace("-", ""),
                Text = Text,
                SoundSelected = GetSoundsByName(SoundSelected),
                From = From,
                To = To,
                EventHour = EventHour,
                Description = Description,
                isEventRepeat = isEventRepeat,
                Status = true,
            };

            if (newItem.isEventRepeat)
            {
                newItem.RepeatEvent = new ItemRepeat()
                {
                    Quantity = Quantity,
                    QuantityType = QuantityType
                };
            }

            await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
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
    }
}
