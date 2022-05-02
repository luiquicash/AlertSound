using AlertSound.Models;
using AlertSound.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private int quantity;
        private string quantityType;

        public NewItemViewModel()
        {
            eventHour = TimeSpan.Parse(DateTime.Now.ToString("hh:mm:ss"));
            FromMinimumDate = DateTime.Now;
            ToMinimumDate = DateTime.Now;
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
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
        public DateTime FromMinimumDate { get; }
        public DateTime ToMinimumDate { get; set; }
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

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
            return !String.IsNullOrWhiteSpace(text);
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
                SoundSelected = GetSounds(SoundSelected),
                From = From,
                To = To,
                EventHour = EventHour,
                Description = Description,
                isEventRepeat = isEventRepeat,
                Status = true,
                RepeatEvent = new ItemRepeat()
                {
                    Quantity = Quantity,
                    QuantityType = QuantityType
                }
            };

            await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private string GetSounds(string itemName)
        {
            string output =string.Empty;
            var response = GetSoundsList().FirstOrDefault(x => x.Name == itemName);
            if (response != null)
                return response.Value;
            else
                return output;
        }
    }
}
