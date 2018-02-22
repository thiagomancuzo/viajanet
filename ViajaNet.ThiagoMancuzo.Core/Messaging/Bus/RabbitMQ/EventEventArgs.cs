using System;
using System.Collections.Generic;
using System.Text;

namespace ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.RabbitMQ
{
    public class EventEventArgs : EventArgs
    {
        public EventEventArgs(string eventName)
        {
            EventName = eventName;
        }

        public string EventName { get; }
    }
}
