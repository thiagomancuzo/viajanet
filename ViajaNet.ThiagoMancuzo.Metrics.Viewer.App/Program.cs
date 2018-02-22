using Autofac;
using System;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Services.Repositories;
using ViajaNet.ThiagoMancuzo.Metrics.Infra.Sessions.Repositories;

namespace ViajaNet.ThiagoMancuzo.Metrics.Viewer.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container;
            var builder = new ContainerBuilder();
            Receiver.Bootstrapper.Bootstrap(builder);
            Core.Bootstrapper.Bootstrap(builder);
            Infra.Bootstrapper.Bootstrap(builder);
            
            container = builder.Build();
            
            var repo = container.Resolve<ISessionMetricsRepository>();
            var avgs = repo.GetSessionAverageByCity();
            if(avgs != null)
            {
                foreach (var avg in avgs)
                {
                    Console.WriteLine("A cidade '{0}' possui uma média de '{1}' sessões por dia", avg.City, avg.Avg);
                }
            }
            

            Console.ReadLine();
        }
    }
}
