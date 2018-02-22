using System;
using System.Collections.Generic;
using System.Text;
using ViajaNet.ThiagoMancuzo.Core.Messaging;

namespace ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Events
{
    public class SessionMetricsReadEvent : Event
    {
        public DateTime Date { get; set; }
        public int SessionCount { get; set; }
        public string City { get; set; }
    }
}
