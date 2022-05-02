using System;

namespace AlertSound.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string SoundSelected { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TimeSpan EventHour { get; set; }
        public bool Status { get; set; }
        public bool isEventRepeat { get; set; }
        public ItemRepeat RepeatEvent { get; set; }
    }

    public class ItemRepeat
    {
        public int Quantity { get; set; }
        public string QuantityType { get; set; }
    }

    public class ItemQuantityType
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}