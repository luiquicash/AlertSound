namespace AlertSound.Services.Abstracts
{
    public interface ICustomNotificationService
    {
        void Initialize();
        void SendNotification(string title, string message);
        void ReceiveNotification(string title, string message);
    }
}
