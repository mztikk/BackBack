using System;

namespace BackBack.Models.Events
{
    public class TriggerEventArgs : TimeEventArgs
    {
        public TriggerEventArgs(DateTime time) : base(time) { }
    }
}
