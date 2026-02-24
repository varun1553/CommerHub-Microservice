using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CommerceHub.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    private const string ExchangeName = "commercehub.exchange";

    public RabbitMqPublisher(RabbitMqSettings settings)
    {
        var factory = new ConnectionFactory()
        {
            HostName = settings.HostName,
            UserName = settings.UserName,
            Password = settings.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: ExchangeName,
            type: ExchangeType.Fanout,
            durable: true
        );
    }

    public Task PublishAsync(string eventName, object message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: "",
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }
}