using AlertSound.Models;
using AlertSound.Services;
using AlertSound.ViewModels;
using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
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

            if (Device.RuntimePlatform == Device.iOS)
            {
                IOSCode();
                StartService();
            }

            if (Device.RuntimePlatform == Device.Android)
            {
                AndroidCode();
                StartService();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        #region DevicePlatform Code
        #region Android
        void AndroidCode()
        {
            MessagingCenter.Subscribe<LocationMessage>(this, "Location", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (ViewModelList.nextDate is null)
                        ViewModelList.nextDate = DateTime.Now.Date;

                    var isNextDay = ViewModelList.nextDate.Value.Date == DateTime.Now.Date;
                    if (isNextDay)
                    {
                        ViewModelList.nextDate = DateTime.Now.AddDays(1);
                        ViewModelList.staticAlarmList = new List<Events>();
                    }

                    GetAlarmDayList();

                    if (DateTime.Now.ToString("ss") == "01" && ViewModelList.staticAlarmList.Count > 0)
                    {
                        var (hasToLaunch, item) = ExceutorAlert();
                        if (hasToLaunch)
                            _viewModel.LaunchAlarm(item);
                    }

                    hourLabel.Text = DateTime.Now.ToString("hh:mm:ss");
                    locationLabel.Text += $"{Environment.NewLine}{message.Latitude}, {message.Longitude}, {DateTime.Now.ToLongTimeString()}";
                    Console.WriteLine($"{message.Latitude}, {message.Longitude}, {DateTime.Now.ToLongTimeString()}");
                });
            });

            MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    locationLabel.Text = "Location Service has been stopped!";
                });
            });

            MessagingCenter.Subscribe<LocationErrorMessage>(this, "LocationError", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    locationLabel.Text = "There was an error updating location!";
                });
            });

            if (Preferences.Get("LocationServiceRunning", false) == true)
            {
                StartService();
            }
        }
        #endregion

        #region iOS
        async void IOSCode()
        {
            if (CrossGeolocator.Current.IsListening)
            {
                await CrossGeolocator.Current.StopListeningAsync();
                CrossGeolocator.Current.PositionChanged -= Current_PositionChanged;

                return;
            }

            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(1), 10, false, new Plugin.Geolocator.Abstractions.ListenerSettings
            {
                ActivityType = Plugin.Geolocator.Abstractions.ActivityType.AutomotiveNavigation,
                AllowBackgroundUpdates = true,
                DeferLocationUpdates = false,
                DeferralDistanceMeters = 10,
                DeferralTime = TimeSpan.FromSeconds(5),
                ListenForSignificantChanges = true,
                PauseLocationUpdatesAutomatically = true
            });

            CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
        }

        void Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            if (ViewModelList.nextDate is null)
                ViewModelList.nextDate = DateTime.Now.Date;

            var isNextDay = ViewModelList.nextDate.Value.Date == DateTime.Now.Date;
            if (isNextDay)
            {
                ViewModelList.nextDate = DateTime.Now.AddDays(1);
                ViewModelList.staticAlarmList = new List<Events>();
            }

            GetAlarmDayList();

            if (DateTime.Now.ToString("ss") == "01" && ViewModelList.staticAlarmList.Count > 0)
            {
                var (hasToLaunch, item) = ExceutorAlert();
                if (hasToLaunch)
                    _viewModel.LaunchAlarm(item);
            }

            hourLabel.Text = DateTime.Now.ToString("hh:mm:ss");
            locationLabel.Text += $"{e.Position.Latitude}, {e.Position.Longitude}, {e.Position.Timestamp.TimeOfDay}{Environment.NewLine}";
            Console.WriteLine($"{e.Position.Latitude}, {e.Position.Longitude}, {e.Position.Timestamp.TimeOfDay}");
        }
        #endregion
        #endregion

        #region Helpers
        private void StartService()
        {
            var startServiceMessage = new StartServiceMessage();
            MessagingCenter.Send(startServiceMessage, "ServiceStarted");
            Preferences.Set("LocationServiceRunning", true);
            locationLabel.Text = "Location Service has been started!";
        }
        public async void GetAlarmDayList()
        {
            if (ViewModelList.staticAlarmList is null || !ViewModelList.staticAlarmList.Any())
            {
                ViewModelList.staticAlarmList = await App.Data.GetEventListsByDayAsync(DateTime.Today.Date);
            }
        }
        private (bool, Events) ExceutorAlert()
        {
            var locShared = new AlertExecutorService();
            return locShared.Excecutor(ViewModelList.staticAlarmList);
        }
        #endregion
    }
}