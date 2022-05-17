using AlertSound.Constants;
using AlertSound.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlertSound.Services
{
    public class AlertExecutorService
    {
        public AlertExecutorService()
        {
        }

        public (bool, Events) Excecutor(List<Events> alarmToday)
        {
            if (alarmToday != null && alarmToday.Any())
            {
                foreach (var item in alarmToday)
                {
                    if (!item.Status && item.IsEventRepeat)
                    {
                        var isRangeItem = item.IsRange && item.To != null && item.To.Value.Date > item.From.Date;
                        double daysDifferent = 0;
                        if (isRangeItem)
                        {
                            TimeSpan time = item.To.Value - item.From;
                            daysDifferent = time.TotalDays;
                        }

                        var daysQuantity = DayToRepeat(item.QuantityType, item.Quantity);
                        var activateDay = isRangeItem ? item.To.Value.Date.AddDays(daysQuantity) : item.From.Date.AddDays(daysQuantity);
                        if (activateDay.Date == DateTime.Now.Date)
                        {
                            item.From = activateDay;
                            if (isRangeItem)
                            {
                                item.To = activateDay.AddDays(daysDifferent);
                            }
                            item.Status = true;
                        }

                        return (false, new Events());
                    }
                    else if (item.Status && (item.To != null && item.To.Value.Date == DateTime.Now.Date
                                         || item.From.Date == DateTime.Now.Date && item.To is null))
                    {
                        var isSoundNow = IsHourToSound(item.EventHour);
                        if (isSoundNow)
                        {
                            item.Status = false;
                            item.LastDayAlarmSound = DateTime.Now;
                            UpdateAlarm(item);
                        }

                        return (true, item);
                    }
                    else if (item.Status && (item.LastDayAlarmSound is null
                                         || item.LastDayAlarmSound.Value.Date != DateTime.Now.Date))
                    {
                        var isSoundNow = IsHourToSound(item.EventHour);
                        if (isSoundNow)
                        {
                            item.LastDayAlarmSound = DateTime.Now;
                            UpdateAlarm(item);
                        }

                        return (true, item);
                    }
                }
            }

            return (false, new Events());
        }

        #region Helpers
        private double DayToRepeat(string type, int quantity)
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

        private bool IsHourToSound(TimeSpan eventHour)
        {
            var currentDay = DateTime.Now;
            var isHourToSound = eventHour.Hours == currentDay.Hour
                             && (eventHour.Minutes == currentDay.AddMinutes(-1).Minute
                             || eventHour.Minutes == currentDay.Minute
                             || eventHour.Minutes == currentDay.AddMinutes(1).Minute);

            return isHourToSound;
        }

        private async void UpdateAlarm(Events item)
        {
            await App.Data.UpdateEventAsync(item);
        }
        #endregion
    }
}
