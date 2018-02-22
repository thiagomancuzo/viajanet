using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.Handlers;

namespace ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.RabbitMQ
{
    public class SubscriptionsManager
    {
        private readonly IDictionary<string, IList<Subscription>> _handlers
            = new Dictionary<string, IList<Subscription>>();

        public bool IsEmpty => !_handlers.Keys.Any();

        public event EventHandler<EventEventArgs> OnEventRemoved;
        public event EventHandler<EventEventArgs> OnEventAdded;

        public void AddSubscription<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            AddSubscription(
                typeof(TEventHandler),
                typeof(TEvent).Name,
                typeof(TEvent)
                );
        }

        private void AddSubscription(Type handlerType, string eventName, Type eventType = null)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<Subscription>());
                OnEventAdded?.Invoke(this, new EventEventArgs(eventName));
            }

            if (((List<Subscription>)_handlers[eventName]).Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(Subscription.New(
                handlerType, eventType)
                );
        }
                
        public void RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {

            var eventName = typeof(TEvent).Name;
            var handlerToRemove = FindSubscriptionToRemove<TEventHandler>(eventName);
            RemoveSubscription(eventName, handlerToRemove);
        }

        Subscription FindSubscriptionToRemove<TEventHandler>(string eventName)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == typeof(TEventHandler));
        }

        private void RemoveSubscription(
            string eventName,
            Subscription subsToRemove
            )
        {
            if (subsToRemove == null) return;
            _handlers[eventName].Remove(subsToRemove);

            if (_handlers[eventName].Any()) return;

            _handlers.Remove(eventName);
            OnEventRemoved?.Invoke(this, new EventEventArgs(eventName));
        }

        public bool HasSubscriptionsForEvent(string eventName) =>
            _handlers.ContainsKey(eventName);

        //public IEnumerable<Subscription> GetHandlersForEvent()
        //{
        //    var key = typeof(TEvent).Name;
        //    return GetHandlersForEvent(key);
        //}

        public IEnumerable<Subscription> GetHandlersForEvent(string eventName)
            => _handlers[eventName];

        public Type GetEventTypeByName(string eventName) => _handlers[eventName]
            ?.FirstOrDefault(handler => !handler.IsDynamic)
            ?.EventType;
    }
}
