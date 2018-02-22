using System;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Events;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus;
using ViajaNet.ThiagoMancuzo.Metrics.Receiver.Sessions.Events;

namespace ViajaNet.ThiagoMancuzo.Metrics.Receiver
{
    public class Worker
    {
        readonly IEventBus _bus;
        public Worker(IEventBus bus)
        {
            _bus = bus;
        }

        public void Run()
        {
            _bus.Subscribe<SessionMetricsReadEvent, SessionMetricsReadEventHandler>();
        }
    }
}
