using MailServer.Services;

namespace MailServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var rabbitMqService = new RabbitMqService();
            rabbitMqService.StartListening();
        }
    }
}