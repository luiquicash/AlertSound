using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace AlertSound.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class AlertPageViewModel : BaseViewModel
    {
        private string description;
        private string text;

        private string itemId;

        public AlertPageViewModel()
        {
            ResumeCommand = new Command(OnResume);
            StopCommand = new Command(OnStop);

            this.PropertyChanged +=
                (_, __) => ResumeCommand.ChangeCanExecute();
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

        public Command ResumeCommand { get; }
        public Command StopCommand { get; }

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await App.Data.GetEventAsync(itemId);
                Id = item.Id;
                Text = item.Text;
                Description = item.Description;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        private async void OnStop()
        {
            var item = await App.Data.GetEventAsync(itemId);

            App.Data.StopAlarm(item.SoundSelected);

            item.IsResume = false;
            item.IsStoped = true;
            await App.Data.UpdateEventAsync(item);

            await Shell.Current.GoToAsync("..");
        }
        private async void OnResume()
        {
            var item = await App.Data.GetEventAsync(itemId);

            item.IsResume = true;
            await App.Data.UpdateEventAsync(item);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
