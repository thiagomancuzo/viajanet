using Autofac;
using Couchbase.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services.Repositories;
using ViajaNet.ThiagoMancuzo.Metrics.Infra.Couchbase;
using ViajaNet.ThiagoMancuzo.Metrics.Infra.Sessions.Repositories;

namespace ViajaNet.ThiagoMancuzo.Metrics.Infra
{
    public static class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<BucketProvider>().As<IBucketProvider>();
            builder.RegisterType<SessionMetricsRepository>().As<ISessionMetricsRepository>();
        }
    }
}
