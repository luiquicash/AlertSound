using System;

namespace AlertSound.Models
{
    public class Item
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}