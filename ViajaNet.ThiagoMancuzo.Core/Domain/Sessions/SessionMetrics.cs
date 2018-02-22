using System;
using System.Collections.Generic;
using System.Text;

namespace ViajaNet.ThiagoMancuzo.Core.Domain.Sessions
{
    public class SessionMetrics
    {
        public SessionMetrics(DateTime date, int sessionsCount, string city)
        {
            Date = date;
            SessionCount = sessionsCount;
            City = city;
        }

        public DateTime Date { get; set; }
        public int SessionCount { get; set; }
        public string City { get; set; }
        public string Type { get => typeof(SessionMetrics).Name; }
    }
}
