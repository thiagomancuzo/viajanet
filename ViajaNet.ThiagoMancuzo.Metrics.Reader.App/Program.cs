using Autofac;
using System;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services;
using ViajaNet.ThiagoMancuzo.Core.Messaging;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.RabbitMQ;
using ViajaNet.ThiagoMancuzo.Metrics.Reader.Services.Sessions;

namespace ViajaNet.ThiagoMancuzo.Metrics.Reader.App
{
    class Program
    {
        
        static void Main(string[] args)
        {
            IContainer container;
            var builder = new ContainerBuilder();
            Core.Bootstrapper.Bootstrap(builder);
            Bootstrapper.Bootstrap(builder);

            container = builder.Build();
            
            using (var _scope = container.BeginLifetimeScope())
            {
                var svc = _scope.Resolve<ISessionMetricsReaderService>();
                var date = new DateTime(2017, 05, 17);
                for (int i = 0; i < 30; i++)
                svc.SendSessionMetricsCommand("150640551", date.AddDays(i));
                Console.ReadLine();
            }

        }
    }
}
