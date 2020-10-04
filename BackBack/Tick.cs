using System;
using System.Threading;
using BackBack.Models.Events;
using BackBack.Storage.Settings;
using Microsoft.Extensions.Logging;
using Stylet;

namespace BackBack
{
    internal class Tick : IDisposable
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Settings _settings;
        private readonly Timer _timer;
        private readonly ILogger _logger;

        public Tick(IEventAggregator eventAggregator, Settings settings, Func<Type, ILogger> loggerCreator)
        {
            _logger = loggerCreator(typeof(Tick));

            _eventAggregator = eventAggregator;
            _settings = settings;

            int interval = 100;
            _logger.LogInformation("Creating {tick} with interval of '{interval}'", nameof(Tick), interval);

            _timer = new Timer(OnTick, null, 0, interval);
        }

        public void Dispose() => ((IDisposable)_timer).Dispose();
        private void OnTick(object? state)
        {
            var tickevent = new TickEvent();
            _logger.LogTrace("Publishing {tickevent} at {now}", nameof(TickEvent), tickevent.Time);
            _eventAggregator.Publish(tickevent);
        }
    }
}
