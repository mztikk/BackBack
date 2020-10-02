using System;

namespace BackBack.Triggers
{
    public abstract class TriggerEvent
    {
        public event EventHandler<EventArgs> OnTrigger;

        protected virtual void Trigger(EventArgs e)
        {
            EventHandler<EventArgs> t = OnTrigger;
            t?.Invoke(this, e);
        }
    }
}
