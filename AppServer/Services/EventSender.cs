using System.Text;
using System.Text.Json;
using AppServer.Domain;
using RabbitMQ.Client;

namespace AppServer.Services;

public static class EventSender
{
    public static ConnectionFactory factory;
    public static IModel channel;

    static EventSender()
    {
        factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: "purchase", type: ExchangeType.Fanout);
    }

    public static void SendPurchaseEvent(Purchase purchase)
    {
        
        var json = JsonSerializer.Serialize(purchase);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: "purchase",
            routingKey: "",
            basicProperties: null,
            body: body);
    }
}
