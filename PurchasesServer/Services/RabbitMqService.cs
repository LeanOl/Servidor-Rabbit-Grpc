using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Connections;
using PurchasesServer.Domain;
using PurchasesServer.Storage;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PurchasesServer.Services;

public class RabbitMqService
{
    public RabbitMqService()
    {

    }

    public void StartListening()
    {
        // receive fanout messages 
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "purchase", type: ExchangeType.Fanout);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                exchange: "purchase",
                routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var purchase = JsonSerializer.Deserialize<Purchase>(json);
                PurchaseDB.Instance.AddPurchase(purchase);
            };
            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
            Console.ReadLine();
        }
    }


}