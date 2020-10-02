using System;
using BackBack.Models.Events;

namespace BackBack.Triggers
{
    public abstract class TriggerEvent
    {
        public event EventHandler<TriggerEventArgs> OnTrigger;

        protected virtual void Trigger(TriggerEventArgs e)
        {
            EventHandler<TriggerEventArgs> t = OnTrigger;
            t?.Invoke(this, e);
        }
    }
}
