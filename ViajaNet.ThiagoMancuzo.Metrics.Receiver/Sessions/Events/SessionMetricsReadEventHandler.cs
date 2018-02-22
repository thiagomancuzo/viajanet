using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Events;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services.Repositories;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.Handlers;

namespace ViajaNet.ThiagoMancuzo.Metrics.Receiver.Sessions.Events
{
    public class SessionMetricsReadEventHandler : IEventHandler<SessionMetricsReadEvent>
    {
        readonly ISessionMetricsRepository _sessionMetricsRepository;

        public SessionMetricsReadEventHandler(ISessionMetricsRepository sessionMetricsRepository)
        {
            this._sessionMetricsRepository = sessionMetricsRepository;
        }

        public Task Handle(SessionMetricsReadEvent @event)
        {
            _sessionMetricsRepository.Save(new SessionMetrics(@event.Date, @event.SessionCount, @event.City));
            Console.WriteLine("{0}  {1} {2}", @event.Date, @event.City, @event.SessionCount);
            return Task.CompletedTask;
        }
    }
}
