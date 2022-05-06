using AlertSound.Models;
using System;
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

        #region Properties
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
        #endregion

        public DateTime FromMinimumDate { get; }
        public DateTime ToMinimumDate { get; set; }
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command PlayCommand { get; }
        public Command StopCommand { get; }

        private string GetSoundsByName(string itemName)
        {
            string output = string.Empty;
            var response = App.Data.GetSoundsList().FirstOrDefault(x => x.Name == itemName);
            if (response != null)
                return response.Value;
            else
                return output;
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
            App.Data.PlayAlarm(soundValue);
            isPlayButtonVisible = false;
            isStopButtonVisible = true;
        }
        private void StopSound()
        {
            if (string.IsNullOrWhiteSpace(SoundSelected))
                return;

            var soundValue = GetSoundsByName(SoundSelected);
            App.Data.StopAlarm(soundValue);
            isPlayButtonVisible = true;
            isStopButtonVisible = false;
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
        private async void OnSave()
        {
            Events newItem = new Events()
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
                Resume = 5,
                IsResume = false
            };

            if (isEventRepeat)
            {
                newItem.Quantity = Quantity;
                newItem.QuantityType = QuantityType;
            }

            await App.Data.AddEventAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
