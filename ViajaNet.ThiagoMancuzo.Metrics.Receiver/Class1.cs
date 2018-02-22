using System;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Events;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus;
using ViajaNet.ThiagoMancuzo.Metrics.Receiver.Sessions.Events;

namespace ViajaNet.ThiagoMancuzo.Metrics.Receiver
{
    public class Class1
    {
        readonly IEventBus _bus;
        public Class1(IEventBus bus)
        {
            _bus = bus;
        }

        public void Run()
        {
            _bus.Subscribe<SessionMetricsReadEvent, SessionMetricsReadEventHandler>();
        }
    }
}
