using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.Handlers;

namespace ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.RabbitMQ
{
    public class Subscription
    {
        public bool IsDynamic => EventType == null;
        public Type HandlerType { get; }
        public Type EventType { get; }

        private Subscription(Type handlerType, Type eventType = null)
        {
            HandlerType = handlerType;
            EventType = eventType;
        }

        public async Task Handle(string message, ILifetimeScope scope)
        {
                var eventData = JsonConvert.DeserializeObject(message, EventType);
                var handler = scope.ResolveOptional(HandlerType);
                var concreteType = typeof(IEventHandler<>).MakeGenericType(EventType);
                await (Task)concreteType.GetMethod("Handle")
                    .Invoke(handler, new[] { eventData });
        }

        public static Subscription New(Type handlerType, Type eventType) =>
            new Subscription(handlerType, eventType);
    }
}
