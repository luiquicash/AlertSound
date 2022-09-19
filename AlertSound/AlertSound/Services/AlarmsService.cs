using AlertSound.Models;
using AlertSound.Services;
using AlertSound.Services.Abstracts;
using AlertSound.ViewModels;
using AlertSound.Views;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AlertSound
{
    public class AlarmsService
    {
        readonly bool stopping = false;
        public AlarmsService()
        {
        }

        public async Task Run(CancellationToken token)
        {
            await Task.Run(async () =>
            {
                var flagAlreadySend = false;
                while (!stopping)
                {
                    token.ThrowIfCancellationRequested();
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10));
                        if (!flagAlreadySend)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                GetAlarmDayList();

                                var (hasToLaunch, item) = ExceutorAlert();
                                if (hasToLaunch)
                                    LaunchAlarm(item);

                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var errormessage = new ErrorMessage();
                            MessagingCenter.Send(errormessage, "AlarmError" + ex);
                        });
                    }
                }
                return;
            }, token);
        }

        #region Helpers
        private async void GetAlarmDayList()
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

        private async void LaunchAlarm(Events item)
        {
            if (item == null)
                return;

            App.Data.PlayAlarm(item, true, true);
            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(AlertPage)}?{nameof(AlertPageViewModel.ItemId)}={item.Id}");
        }
        #endregion
    }
}

