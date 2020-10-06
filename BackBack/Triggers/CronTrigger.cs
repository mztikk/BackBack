using System;
using BackBack.Models;
using BackBack.Models.Events;
using Cronos;
using Microsoft.Extensions.Logging;
using RF.WPF.Extensions;
using Stylet;

namespace BackBack.Triggers
{
    public class CronTrigger : TriggerEvent, IHandle<TickEvent>, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IEventAggregator _eventAggregator;
        private bool _disposedValue;

        public CronTrigger(IEventAggregator eventAggregator, Func<Type, ILogger> loggerFactory)
        {
            _logger = loggerFactory(typeof(CronTrigger));

            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
        }

        public BackupItem BackupItem { get; set; }

        private CronExpression _cronExpression;
        public CronExpression CronExpression
        {
            get => _cronExpression; set
            {
                _cronExpression = value;
                NextExecution = _cronExpression.GetNextOccurrence(new DateTimeOffset(BackupItem.LastExecution), TimeZoneInfo.Local, true);
            }
        }

        private DateTimeOffset? _nextExecution;
        private DateTimeOffset? NextExecution
        {
            get => _nextExecution; set
            {
                _nextExecution = value;
                _logger.LogInformation("Next Execution of '{name}' set to {value}", BackupItem.Name, value);
            }
        }

        public void Handle(TickEvent message)
        {
            if (message.Time >= NextExecution)
            {
                _logger.LogDebug("{type} triggering with {next} at {messageTime}", this.TypeName(), NextExecution, message.Time);
                Trigger(new TriggerEventArgs(message.Time));
                NextExecution = _cronExpression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local, true);
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
