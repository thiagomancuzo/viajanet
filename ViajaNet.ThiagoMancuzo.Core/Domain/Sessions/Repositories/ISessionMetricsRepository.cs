using System;
using System.Collections.Generic;
using System.Text;
using ViajaNet.ThiagoMancuzo.Metrics.Domain.Sessions.DTO;

namespace ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services.Repositories
{
    public interface ISessionMetricsRepository
    {
        void Save(SessionMetrics sessionMetrics);
        IEnumerable<SessionsPerCityAvgOutput> GetSessionAverageByCity();
    }
}
