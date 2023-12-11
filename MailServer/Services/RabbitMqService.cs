using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using MailServer.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MailServer.Services;

public class RabbitMqService
{
    private readonly ConcurrentQueue<Purchase> _messageQueue = new ConcurrentQueue<Purchase>();
    private bool serverOn = true;

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
            consumer.Received +=  (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var purchase = JsonSerializer.Deserialize<Purchase>(json);
                _messageQueue.Enqueue(purchase);

            };
            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
            Task.Run(SendEmail);
            Console.ReadLine();
            serverOn = false;
        }

    }
    private async Task SendEmail()
    {
        while (serverOn)
        {
            if (_messageQueue.TryDequeue(out var purchase))
            {
                Console.WriteLine($"Enviando correo a usuario: {purchase.Username}");
                await Task.Delay(5000);
            }
        }
    }



}