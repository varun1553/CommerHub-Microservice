namespace CommerceHub.Infrastructure.Messaging;

public class RabbitMqSettings
{
    public string HostName { get; set; } = "localhost";
    public string ExchangeName { get; set; } = "commercehub.exchange";
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
