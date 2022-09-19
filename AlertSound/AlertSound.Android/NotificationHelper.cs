using AlertSound.Services.Abstracts;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using System;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(AlertSound.Droid.NotificationHelper))]
namespace AlertSound.Droid
{
    public class NotificationHelper : ICustomNotificationService
    {
        private static string ForegroundChannelId = "9001";
        public static NotificationHelper Instance { get; private set; }
        public NotificationManager Manager { get; private set; }

        public bool ChannelInitialized { get; private set; }
        public int MessageId { get; set; } = 0;
        public int PendingIntentId { get; set; } = 0;

        public NotificationHelper() => Initialize();
        public void Initialize()
        {
            if (Instance == null)
            {
                CreateNotificationChannel();
                Instance = this;
            }
        }

        public void SendNotification(string title, string message)
        {
            if (!ChannelInitialized)
            {
                CreateNotificationChannel();
            }

            if (ChannelInitialized)
                Show(title, message);
        }

        public void ReceiveNotification(string title, string message)
        {
            throw new System.NotImplementedException();
        }

        private void CreateNotificationChannel()
        {
            Manager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
            if (Manager is null)
            {
                ChannelInitialized = false;
                return;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ForegroundChannelId, "Title", NotificationImportance.High);
                channel.Importance = NotificationImportance.High;
                channel.EnableLights(true);
                channel.EnableVibration(true);
                channel.SetShowBadge(true);
                channel.SetVibrationPattern(new long[] { 100, 200, 300 });

                Manager.CreateNotificationChannel(channel);
            }

            ChannelInitialized = true;
        }

        public void Show(string title, string message)
        {
            Intent intent = new Intent(Application.Context, typeof(MainActivity));
            intent.PutExtra("Title", title);
            intent.PutExtra("Message", message);

            var pendingIntent = PendingIntent.GetActivity(Application.Context, PendingIntentId++, intent, PendingIntentFlags.UpdateCurrent);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context, ForegroundChannelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.navigation_empty_icon)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

            Notification notification = builder.Build();
            Manager.Notify(MessageId++, notification);
        }


        [Obsolete]
        public Notification GetServiceStartedNotification()
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.PutExtra("Title", "Message");

            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent);

            var notificationBuilder = new NotificationCompat.Builder(Application.Context, ForegroundChannelId)
                .SetContentTitle("Alarm Started")
                .SetContentText("")
                .SetSmallIcon(Resource.Drawable.navigation_empty_icon)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel notificationChannel = new NotificationChannel(ForegroundChannelId, "Title", NotificationImportance.High);
                notificationChannel.Importance = NotificationImportance.High;
                notificationChannel.EnableLights(true);
                notificationChannel.EnableVibration(true);
                notificationChannel.SetShowBadge(true);
                notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300 });

                var notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
                if (notificationManager != null)
                {
                    notificationBuilder.SetChannelId(ForegroundChannelId);
                    notificationManager.CreateNotificationChannel(notificationChannel);
                }
            }

            return notificationBuilder.Build();
        }
    }
}

