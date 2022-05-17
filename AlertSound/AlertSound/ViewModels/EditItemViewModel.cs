using AlertSound.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
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
        private DateTime fromminday;
        private DateTime to;
        private TimeSpan eventHour;

        private bool status;
        private bool isrange;
        private bool iseventrepeat;

        private int quantity;
        private string quantityType;

        private bool isplaybuttonvisible;
        private bool isstopbuttonvisible;

        private int indexsound;
        private int indexquantity;
        private string itemId;

        public EditItemViewModel()
        {
            isPlayButtonVisible = true;
            isStopButtonVisible = false;
            eventHour = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
            SaveCommand = new Command(OnUpdate, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PlayCommand = new Command(PlaySound);
            StopCommand = new Command(StopSound);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        #region Properties
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
        public bool isRange
        {
            get => isrange;
            set => SetProperty(ref isrange, value);
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
        public DateTime FromMinDay
        {
            get => fromminday;
            set => SetProperty(ref fromminday, value);
        }
        #endregion

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command PlayCommand { get; }
        public Command StopCommand { get; }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(text);
        }
        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await App.Data.GetEventAsync(itemId);
                Id = item.Id;
                Text = item.Text.ToAllFirstLetterInUpper();
                Description = item.Description.ToAllFirstLetterInUpper();
                SoundSelected = GetSoundsByValue(item.SoundSelected);
                From = item.From.Date;
                FromMinDay = item.From.Date.AddDays(-1);
                if (item.To != null)
                    To = item.To.Value.Date;
                isRange = item.To != null && item.To.Value.Date > item.From.Date || item.IsRange;
                EventHour = item.EventHour;
                isEventRepeat = item.IsEventRepeat;
                Status = item.Status;
                Quantity = item.Quantity;
                QuantityType = item.QuantityType;
                IndexSound = GetIndexSoundByValue(item.SoundSelected);
                IndexQuantity = GetIndexQuantityByValue(item.QuantityType);
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
        private string GetSoundsByValue(string itemValue)
        {
            string output = string.Empty;
            var response = App.Data.GetSoundsList().FirstOrDefault(x => x.Value == itemValue);
            if (response != null)
                return response.Name;
            else
                return output;
        }
        private string GetSoundsByName(string itemName)
        {
            string output = string.Empty;
            var response = App.Data.GetSoundsList().FirstOrDefault(x => x.Name == itemName);
            if (response != null)
                return response.Value;
            else
                return output;
        }
        private int GetIndexSoundByValue(string itemValue)
        {
            int output = 0;
            var index = App.Data.GetSoundsList().Select(x => x.Value).ToList().FindIndex(a => a.Contains(itemValue));
            if (index > 0)
                return index;
            else
                return output;
        }
        private int GetIndexQuantityByValue(string itemName)
        {
            int output = 0;
            var index = App.Data.GetQuantityTypes().Select(x => x.Name).ToList().FindIndex(a => a.Contains(itemName));
            if (index > 0)
                return index;
            else
                return output;
        }
        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
        private async void OnUpdate()
        {
            var item = await App.Data.GetEventAsync(itemId);

            item.Text = Text.ToAllFirstLetterInUpper();
            item.From = From;
            if (isRange)
                item.To = To;
            item.EventHour = EventHour;
            item.Description = Description.ToAllFirstLetterInUpper();
            item.SoundSelected = GetSoundsByName(SoundSelected);
            item.IsEventRepeat = isEventRepeat;
            item.Status = Status;

            if (isEventRepeat)
            {
                item.Quantity = Quantity;
                item.QuantityType = QuantityType;
            }

            await App.Data.UpdateEventAsync(item);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
