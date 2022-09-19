using AlertSound.Services.Abstracts;
using AlertSound.ViewModels;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AlertSound.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;
        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ItemsViewModel();

            MessagingCenter.Subscribe<AlarmMessage>(this, "Alarm", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Console.WriteLine($"{message.Title}, {message.Date}, {DateTime.Now.ToLongTimeString()}");
                });
            });

            MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    locationLabel.Text = "Alarm Service has been stopped!";
                });
            });

            MessagingCenter.Subscribe<ErrorMessage>(this, "AlarmError", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    locationLabel.Text = "There was an error updating alarm!";
                });
            });

            if (Preferences.Get("AlarmServiceRunning", false) == true)
            {
                StartService();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        #region Helpers
        private void StartService()
        {
            var notification = DependencyService.Get<ICustomNotificationService>();
            notification.SendNotification("Start", "Alarm Service has been started!");
            
            var startServiceMessage = new StartServiceMessage();
            MessagingCenter.Send(startServiceMessage, "ServiceStarted");
            Preferences.Set("AlarmServiceRunning", true);
            locationLabel.Text = "Alarm Service has been started!";
        }
        #endregion
    }
}