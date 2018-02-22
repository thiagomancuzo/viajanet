using System;
using System.Collections.Generic;
using System.Text;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.Handlers;

namespace ViajaNet.ThiagoMancuzo.Core.Messaging.Bus
{
    public interface IEventBus
    {
        void Publish(Event @event);

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>;
        
        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>;
    }
}
