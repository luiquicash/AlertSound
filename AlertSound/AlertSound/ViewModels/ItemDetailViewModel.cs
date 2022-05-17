using AlertSound.Extensions;
using System;
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
        private bool isrange;
        private bool isrepeatevent;
        private DateTime from;
        private DateTime to;
        private TimeSpan hour;
        public string status;
        public string iseventrepeat;
        private int quantity;
        private string quantityType;

        #region Properties
        public string Id { get; set; }
        public string StatusStr
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        public bool isRange
        {
            get => isrange;
            set => SetProperty(ref isrange, value);
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
        #endregion

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
                if (item.To != null)
                    To = item.To.Value.Date;
                Hour = item.EventHour;
                isRange = item.To != null && item.To.Value.Date > item.From.Date || item.IsRange;
                isEventRepeat = item.IsEventRepeat;
                if (item.IsEventRepeat)
                {
                    Quantity = item.Quantity;
                    QuantityType = item.QuantityType;
                }
                isEventRepeatStr = item.IsEventRepeat ? "Si" : "No";
                StatusStr = item.Status ? "Activo" : "InActivo";
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
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
    }
}
