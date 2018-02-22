using Autofac;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ViajaNet.ThiagoMancuzo.Core.Loggin;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.Handlers;
using ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.RabbitMQ;

namespace ViajaNet.ThiagoMancuzo.Core.Messaging
{
    public class RabbitMQEventBus : IDisposable, IEventBus
    {
        private readonly ILifetimeScope _autofac;
        private readonly PersisterConnection _connection;
        private readonly ILogger _logger;
        private readonly SubscriptionsManager _subscriptionManager;

        private string _queueName;
        private IModel _consumerChannel;

        public string ExchangeName { get; }

        public RabbitMQEventBus(
            PersisterConnection connection,
            ILogger logger,
            ILifetimeScope autofac,
            string exchangeName = "DefaultExchange"
            )
        {

            _connection = connection
                ?? throw new ArgumentNullException(nameof(connection));

            _logger = logger;

            _autofac = autofac
                ?? throw new ArgumentNullException(nameof(autofac));

            ExchangeName = exchangeName;

            _subscriptionManager = new SubscriptionsManager();
            _subscriptionManager.OnEventRemoved += OnSubscriptionManagerEventRemoved;
            _subscriptionManager.OnEventAdded += OnSubscriptionManagerEventAdded;

            _consumerChannel = CreateConsumerChannel();
        }

        void OnSubscriptionManagerEventAdded(object _, EventEventArgs @event)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.QueueBind(
                    queue: _queueName,
                    exchange: ExchangeName,
                    routingKey: @event.EventName
                    );
            }
        }

        void OnSubscriptionManagerEventRemoved(object _, EventEventArgs @event)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using (var channel = _connection.CreateModel())
            {
                channel.QueueUnbind(
                    queue: _queueName,
                    exchange: ExchangeName,
                    routingKey: @event.EventName
                );

                if (!_subscriptionManager.IsEmpty) return;

                _queueName = string.Empty;
                _consumerChannel.Close();
            }
        }

        public void Publish(Event @event)
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger?.LogWarning(ex.ToString());
                });

            using (var channel = _connection.CreateModel())
            {
                var eventName = @event.GetType()
                    .Name;

                channel.ExchangeDeclare(exchange: ExchangeName,
                    type: "direct");

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    channel.BasicPublish(exchange: ExchangeName,
                        routingKey: eventName,
                        basicProperties: null,
                        body: body);
                });
            }
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            _subscriptionManager.AddSubscription<TEvent, TEventHandler>();
        }


        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            _subscriptionManager.RemoveSubscription<TEvent, TEventHandler>();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");
            _queueName = channel.QueueDeclare().QueueName;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await HandleEvent(eventName, message);
            };

            channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private async Task HandleEvent(string eventName, string message)
        {
            if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                return;
            }

            using (var scope = _autofac.BeginLifetimeScope("RabbitMQEventBus"))
            {
                var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    await subscription.Handle(message, scope);
                }
            }
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
        }
    }
}
