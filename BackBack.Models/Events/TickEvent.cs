using System;

namespace BackBack.Models.Events
{
    public class TickEvent : EventArgs
    {
        public TickEvent()
        {
            Time = DateTime.Now;
            UtcTime = DateTime.UtcNow;
            TimeOffset = DateTimeOffset.Now;
        }

        public DateTime Time { get; }
        public DateTime UtcTime { get; }
        public DateTimeOffset TimeOffset { get; }
    }
}
