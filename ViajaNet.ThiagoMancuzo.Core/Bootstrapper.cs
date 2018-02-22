using Autofac;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using ViajaNet.ThiagoMancuzo.Core.Domain.Sessions.Events;
using ViajaNet.ThiagoMancuzo.Core.Loggin;
using ViajaNet.ThiagoMancuzo.Core.Messaging;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.Handlers;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.RabbitMQ;

namespace ViajaNet.ThiagoMancuzo.Core
{
    public static class Bootstrapper
    {
        public static ContainerBuilder builder { get; private set; }
        public static void Bootstrap(ContainerBuilder builder)
        {
            if (builder == null)
                builder = new ContainerBuilder();

            builder.RegisterType<ConnectionFactory>().As<IConnectionFactory>();
            builder.RegisterType<ConsoleLogger>().As<ILogger>();
            builder.RegisterType<RabbitMQEventBus>().As<IEventBus>();
            builder.RegisterType<PersisterConnection>();
        }
    }
}
