using AlertSound.Constants;
using System;
using System.Linq;

namespace AlertSound.Services
{
    public static class AlertExecutor
    {
        public static async void Excecutor()
        {
            var alarmToday = await App.Data.GetEventListsByDayAsync(DateTime.Today);
            if (alarmToday != null && alarmToday.Any())
            {
                foreach (var item in alarmToday)
                {
                    if (!item.Status && item.isEventRepeat)
                    {
                        var daysQuantity = DayToRepeat(item.QuantityType, item.Quantity);
                        var activateDay = item.To.AddDays(daysQuantity);
                        if (activateDay.Date == DateTime.Today.Date)
                        {
                            item.Status = true;
                            await App.Data.UpdateEventAsync(item);
                        }
                    }
                    else if (item.Status && item.To.Date == DateTime.Today.Date)
                    {
                        var isSoundNow = isHourToSound(item.EventHour);
                        if (isSoundNow)
                        {
                            App.Data.PlayAlarm(item);

                            if (!item.IsResume)
                            {
                                item.Status = false;
                                item.IsResume = false;
                                await App.Data.UpdateEventAsync(item);
                            }
                            else
                            {
                                var newHour = DateTime.Now.AddMinutes(item.Resume);
                                item.EventHour = TimeSpan.Parse(newHour.ToString("HH:mm"));
                                item.IsResume = true;
                                await App.Data.UpdateEventAsync(item);
                            }
                        }
                    }
                    else if (item.Status)
                    {
                        var isSoundNow = isHourToSound(item.EventHour);
                        if (isSoundNow)
                            App.Data.PlayAlarm(item);
                    }
                }
            }
        }


        #region Helpers
        private static double DayToRepeat(string type, int quantity)
        {
            double newQuantity = 0;
            switch (type)
            {
                case QuantityTypeConstants.Days:
                    newQuantity = quantity * 1;
                    return newQuantity;
                case QuantityTypeConstants.Week:
                    newQuantity = quantity * 2;
                    return newQuantity;
                case QuantityTypeConstants.Month:
                    newQuantity = quantity * 30;
                    return newQuantity;
                case QuantityTypeConstants.Years:
                    newQuantity = quantity * 365;
                    return newQuantity;
            }
            return newQuantity;
        }

        private static bool isHourToSound(TimeSpan eventHour)
        {
            var isHourToSound = eventHour.Hours == DateTime.Today.Hour && eventHour.Minutes == DateTime.Today.Minute;
            return isHourToSound;
        }
        #endregion

    }
}
