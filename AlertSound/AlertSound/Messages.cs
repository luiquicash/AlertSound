using System;

namespace AlertSound
{
    public class StartServiceMessage
    {
    }

    public class StopServiceMessage
    {
    }

    public class ErrorMessage
    {
    }

    public class AlarmMessage
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }

    }

    public struct MessagesKeys
    {
        public const string Alarm = "AlarmToSound";
    }
}

