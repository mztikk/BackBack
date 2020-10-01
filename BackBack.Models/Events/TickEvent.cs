using System;

namespace BackBack.Models.Events
{
    public class TickEvent : EventArgs
    {
        public TickEvent(DateTime time) => Time = time;

        public DateTime Time { get; }
    }
}
