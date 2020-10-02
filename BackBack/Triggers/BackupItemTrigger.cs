using System;
using BackBack.Models;
using BackBack.Models.Events;
using Stylet;

namespace BackBack.Triggers
{
    public class BackupItemTrigger : TriggerEvent, IHandle<PostBackupEvent>, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;
        private bool _disposedValue;

        public BackupItemTrigger(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public BackupItem BackupItem { get; set; }

        public void Handle(PostBackupEvent message)
        {
            if (message.BackupItem == BackupItem)
            {
                Trigger(new EventArgs());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _eventAggregator.Unsubscribe(this);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BackupItemTrigger()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
