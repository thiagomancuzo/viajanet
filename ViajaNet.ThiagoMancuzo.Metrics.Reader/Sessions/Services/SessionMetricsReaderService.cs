using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Events;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus;
using ViajaNet.ThiagoMancuzo.Metrics.Reader.Google.AnalyticsReporting.Credentials;

namespace ViajaNet.ThiagoMancuzo.Metrics.Reader.Services.Sessions
{
    public class SessionMetricsReaderService : ISessionMetricsReaderService
    {
        readonly ICredentialsProvider _credentialsProvider;
        readonly IEventBus _bus;

        public SessionMetricsReaderService(ICredentialsProvider credentialsProvider,
            IEventBus bus)
        {
            this._credentialsProvider = credentialsProvider;
            this._bus = bus;
        }

        public void SendSessionMetricsCommand(string viewID, DateTime date)
        {
            foreach(var metrics in GetSessionMetrics(viewID, date))
            {
                var command = new SessionMetricsReadEvent()
                {
                    Date = metrics.Date,
                    SessionCount = metrics.SessionCount,
                    City = metrics.City
                };

                _bus.Publish(command);
            }
        }

        public IEnumerable<SessionMetrics> GetSessionMetrics(string viewID, DateTime date)
        {
            using (var svc = new AnalyticsReportingService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = _credentialsProvider.GetCredential(),
                    ApplicationName = "ViajaNet Developer Test"
                }))
            {
                var dateRange = new DateRange
                {
                    StartDate = date.ToString("yyyy-MM-dd"),
                    EndDate = date.ToString("yyyy-MM-dd")
                };
                var sessions = new Metric[]
                {
                    new Metric {
                        Expression = "ga:sessions",
                        Alias = "Sessions"
                    },
                };
                
                var dimension = new Dimension[] 
                {
                    new Dimension { Name = "ga:date" },
                    new Dimension { Name = "ga:city" }
                };

                var reportRequest = new ReportRequest
                {
                    DateRanges = new List<DateRange> { dateRange },
                    Dimensions = dimension,
                    Metrics = sessions.ToList(),
                    ViewId = viewID
                };
                var getReportsRequest = new GetReportsRequest
                {
                    ReportRequests = new List<ReportRequest> { reportRequest }
                };
                var batchRequest = svc.Reports.BatchGet(getReportsRequest);
                var response =  batchRequest.Execute();
                if(response.Reports.First().Data.Rows != null)
                {
                    foreach(var row in response.Reports.First().Data.Rows)
                    {
                        var dateString = row.Dimensions[0];
                        var year = int.Parse(dateString.Substring(0, 4));
                        var month = int.Parse(dateString.Substring(4, 2));
                        var day = int.Parse(dateString.Substring(6, 2));

                        var city = row.Dimensions[1];
                        var sessionCount = int.Parse(row.Metrics.First().Values[0]);

                        yield return new SessionMetrics(new DateTime(year, month, day), sessionCount, city);
                    }
                }
            }
        }

    }
}
