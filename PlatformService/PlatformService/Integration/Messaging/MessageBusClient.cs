using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using PlatformService.Dto;
using RabbitMQ.Client;

namespace PlatformService.Integration.Messaging;

public sealed class MessageBusClient(
    IConfiguration configuration,
    ILogger<MessageBusClient> logger) :
    IMessageBusClient,
    IAsyncDisposable
{
    private const string ExchangeName = "platforms.fanout";

    private readonly SemaphoreSlim _initSemaphore = new(1, 1);
    private readonly SemaphoreSlim _publishSemaphore = new(1, 1);

    private readonly ConnectionFactory _connectionFactory = new()
    {
        HostName = configuration["RabbitMQ:Host"]!,
        Port = configuration.GetValue<int>("RabbitMQ:Port"),
        AutomaticRecoveryEnabled = true
    };

    private IConnection? _connection;
    private IChannel? _channel;

    public async Task PublishPlatform(PlatformPublishDto platform, string eventCause)
    {
        await EnsureChannelAsync();

        await _publishSemaphore.WaitAsync();
        try
        {
            if (!_channel.IsOpen)
            {
                logger.LogError("Cannot send message. RabbitMQ channel/connection not open");
                throw new InvalidOperationException("RabbitMQ connection is not open");
            }

            var message = JsonSerializer.Serialize(platform);
            var body = Encoding.UTF8.GetBytes(message);
            await _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: string.Empty,
                mandatory: false,
                body: body,
                basicProperties: new BasicProperties { Type = eventCause });

            logger.LogInformation(
                "{Event} message was published for platform with ID: {ID}",
                eventCause,
                platform.Id);
        }
        finally
        {
            _publishSemaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.DisposeAsync();
    }

    [MemberNotNull(nameof(_channel), nameof(_connection))]
    private async Task EnsureChannelAsync()
    {
        await _initSemaphore.WaitAsync();
        try
        {
            if (_connection is not null && _channel is not null)
                return;

            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(type: ExchangeType.Fanout, exchange: ExchangeName);

            logger.LogInformation("Connection and channel created. Exchange declared.");
        }
        finally
        {
            _initSemaphore.Release();
        }
    }
}