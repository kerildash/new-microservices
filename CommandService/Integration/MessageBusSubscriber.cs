using System.Diagnostics.CodeAnalysis;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.Integration;

public sealed class MessageBusSubscriber(
    IConfiguration configuration,
    MessageHandler messageHandler,
    ILogger<MessageBusSubscriber> logger) :
    BackgroundService,
    IAsyncDisposable
{
    private const string ExchangeName = "platforms.fanout";

    private readonly SemaphoreSlim _initSemaphore = new(1, 1);

    private readonly ConnectionFactory _connectionFactory = new()
    {
        HostName = configuration["RabbitMQ:Host"]!,
        Port = configuration.GetValue<int>("RabbitMQ:Port"),
        AutomaticRecoveryEnabled = true
    };

    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;

    public async ValueTask DisposeAsync()
    {
        _initSemaphore.Dispose();

        if (_connection is not null)
            await _connection.DisposeAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        await Initialize();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            var messageType = eventArgs.BasicProperties.Type;
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            await messageHandler.ProcessMessage(message, messageType);
        };

        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer, stoppingToken);

        logger.LogInformation("Started to listen on \"{Queue}\" queue.", _queueName);
    }

    [MemberNotNull(nameof(_channel), nameof(_connection), nameof(_queueName))]
    private async Task Initialize()
    {
        await _initSemaphore.WaitAsync();
        try
        {
            _connection = await _connectionFactory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(type: ExchangeType.Fanout, exchange: ExchangeName);

            var queue = await _channel.QueueDeclareAsync();
            _queueName = queue.QueueName;
            await _channel.QueueBindAsync(
                queue: _queueName,
                exchange: ExchangeName,
                routingKey: string.Empty);
        }
        finally
        {
            _initSemaphore.Release();
        }
    }
}