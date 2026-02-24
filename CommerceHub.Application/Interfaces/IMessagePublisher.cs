public interface IMessagePublisher
{
    Task PublishAsync(string eventName, object message);
}