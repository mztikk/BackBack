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
            _logger.LogDebug("Creating {tick} with interval of '{interval}'", nameof(Tick), interval);

            _timer = new Timer(OnTick, null, 0, interval);
        }

        public void Dispose() => ((IDisposable)_timer).Dispose();
        private void OnTick(object? state)
        {
            DateTime now = DateTime.Now;
            var tickevent = new TickEvent(now);
            _logger.LogTrace("Publishing {tickevent} at {now}", nameof(TickEvent), now);
            _eventAggregator.Publish(tickevent);
        }
    }
}
