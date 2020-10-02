using System;

namespace BackBack.Models.Events
{
    public class TimeEventArgs
    {
        public DateTime Time { get; set; }

        public TimeEventArgs(DateTime time) => Time = time;
    }
}
