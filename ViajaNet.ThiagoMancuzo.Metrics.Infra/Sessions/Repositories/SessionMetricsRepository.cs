using Couchbase;
using Couchbase.Core;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services.Repositories;
using ViajaNet.ThiagoMancuzo.Metrics.Domain.Sessions.DTO;

namespace ViajaNet.ThiagoMancuzo.Metrics.Infra.Sessions.Repositories
{
    public class SessionMetricsRepository : ISessionMetricsRepository
    {
        readonly IBucket _bucket;

        public SessionMetricsRepository(IBucketProvider bucketProvider)
        {
            this._bucket = bucketProvider.GetBucket("ViajaNet");
        }

        public void Save(SessionMetrics sessionMetrics)
        {
            _bucket.Upsert(new Document<SessionMetrics>
            {
                Content = sessionMetrics,
                Id = Guid.NewGuid().ToString()
            });
        }

        public IEnumerable<SessionsPerCityAvgOutput> GetSessionAverageByCity()
        {
            var query = new ViewQuery().From("sessions", "by_city");
            var averages = _bucket.Query<dynamic>(query);
            foreach (var average in averages.Rows)
            {
                IEnumerable<SessionsPerCityAvgOutput> @out = JsonConvert.DeserializeObject<IEnumerable<SessionsPerCityAvgOutput>>(average.Value.ToString());
                return @out;
            }

            return null;
        }
    }
}
