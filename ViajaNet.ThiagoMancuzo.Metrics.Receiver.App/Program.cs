using Autofac;
using System;

namespace ViajaNet.ThiagoMancuzo.Metrics.Receiver.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container;
            var builder = new ContainerBuilder();
            Core.Bootstrapper.Bootstrap(builder);
            Bootstrapper.Bootstrap(builder);
            Infra.Bootstrapper.Bootstrap(builder);

            container = builder.Build();

            using (var _scope = container.BeginLifetimeScope())
            {
                var _svc = _scope.Resolve<Class1>();
                _svc.Run();
                Console.ReadLine();
            }
        }
    }
}
