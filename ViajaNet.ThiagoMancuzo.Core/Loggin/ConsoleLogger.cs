using System;
using System.Collections.Generic;
using System.Text;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.RabbitMQ;

namespace ViajaNet.ThiagoMancuzo.Core.Loggin
{
    public class ConsoleLogger : ILogger
    {
        public void LogCritical(string log)
        {
            Console.WriteLine(log);
        }

        public void LogInformation(string log)
        {
            Console.WriteLine(log);
        }

        public void LogWarning(string log)
        {
            Console.WriteLine(log);
        }
    }
}
