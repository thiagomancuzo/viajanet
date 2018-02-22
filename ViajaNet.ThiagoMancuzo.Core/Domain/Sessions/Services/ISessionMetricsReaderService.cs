using System;
using System.Collections.Generic;

namespace ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services
{
    public interface ISessionMetricsReaderService
    {
        IEnumerable<SessionMetrics> GetSessionMetrics(string viewID, DateTime date);
        void SendSessionMetricsCommand(string viewID, DateTime date);
    }
}
