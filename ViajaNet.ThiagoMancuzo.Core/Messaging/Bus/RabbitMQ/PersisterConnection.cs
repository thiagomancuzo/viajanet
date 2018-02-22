using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ViajaNet.ThiagoMancuzo.Core.Loggin;

namespace ViajaNet.ThiagoMancuzo.Core.Messaging.Bus.RabbitMQ
{
    public delegate Policy PersisterConnectionPolicyFactory(ILogger logger);

public class PersisterConnection :
    IDisposable
{
    public static readonly PersisterConnectionPolicyFactory DefaultPolicyFactory = (logger) =>
    {
        return Policy.Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) => { logger?.LogWarning(ex.ToString()); }
            );
    };

    private readonly IConnectionFactory _factory;
    private readonly ILogger _logger;
    private readonly PersisterConnectionPolicyFactory _policyFactory;

    public PersisterConnection(
        IConnectionFactory factory,
        ILogger logger
        )
    {
        _factory = factory;
        _logger = logger;
        _policyFactory = DefaultPolicyFactory;
    }

    private IConnection _connection;
    public bool IsConnected => 
        _connection != null && 
        _connection.IsOpen && 
        !Disposed;

    private readonly object _lockObject = new object();
        

    public bool TryConnect()
    {
        _logger?.LogInformation("RabbitMQ Client is trying to connect");

        lock (_lockObject)
        {
            if (IsConnected) return true;

            var policy = _policyFactory(_logger);
            policy.Execute(() =>
            {
                _connection = _factory.CreateConnection();
            });

            if (!IsConnected)
            {
                _logger?.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                return false;
            }

            WatchConnectionHealth();

            _logger?.LogInformation($"New connection to {_connection.Endpoint.HostName}.");

            return true;
        }
    }

    private void WatchConnectionHealth()
    {
        _connection.ConnectionShutdown += (sender, e) =>
        {
            if (Disposed) return;
            _logger?.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            TryConnect();
        };

        _connection.CallbackException += (sender, e) =>
        {
            if (Disposed) return;
            _logger?.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            TryConnect();
        };

        _connection.ConnectionBlocked += (sender, e) =>
        {
            if (Disposed) return;
            _logger?.LogWarning("A RabbitMQ connection is blocked. Trying to re-connect...");
            TryConnect();
        };
    }

    public IModel CreateModel()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException(
                "There are RabbitMQ connections available to perform this action"
                );
        }

        return _connection.CreateModel();
    }

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;

        try
        {
            _connection.Dispose();
        }
        catch (IOException ex)
        {
            _logger?.LogCritical(ex.ToString());
        }
    }
    public bool Disposed { get; private set; }
}
}
