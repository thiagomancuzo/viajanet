using Autofac;
using Couchbase.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Events;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services.Repositories;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.Handlers;
using ViajaNet.ThiagoMancuzo.Metrics.Receiver.Sessions.Events;

namespace ViajaNet.ThiagoMancuzo.Metrics.Receiver
{
    public static class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<Worker>();
            builder.RegisterType<SessionMetricsReadEventHandler>().As<IEventHandler<SessionMetricsReadEvent>>();
            builder.RegisterType<SessionMetricsReadEventHandler>();
        }
    }
}
