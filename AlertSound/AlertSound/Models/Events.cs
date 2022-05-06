using SQLite;
using System;

namespace AlertSound.Models
{
    public class Events
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string SoundSelected { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TimeSpan EventHour { get; set; }

        public int Resume { get; set; }
        public int Quantity { get; set; }
        public string QuantityType { get; set; }

        public bool Status { get; set; }
        public bool IsResume { get; set; }
        public bool IsStoped { get; set; }
        public bool isEventRepeat { get; set; }
    }
}