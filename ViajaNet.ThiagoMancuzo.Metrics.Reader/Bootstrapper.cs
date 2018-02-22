using Autofac;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services;
using ViajaNet.ThiagoMancuzo.Metrics.Reader.Google.AnalyticsReporting.Credentials;
using ViajaNet.ThiagoMancuzo.Metrics.Reader.Services.Sessions;

namespace ViajaNet.ThiagoMancuzo.Metrics.Reader
{
    public static class Bootstrapper
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            builder.RegisterType<SessionMetricsReaderService>().As<ISessionMetricsReaderService>();
            builder.RegisterType<CredentialsProvider>().As<ICredentialsProvider>();
        }
    }
}
