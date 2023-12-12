using AppServer;
using GrpcServer.Services;

namespace GrpcServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
            // Add services to the container.
            builder.Services.AddGrpc();

            Server server = new Server();
            StartServer(server);

            var app = builder.Build();

            app.Urls.Add("http://localhost:5240");
            app.Urls.Add("https://localhost:5241");
            // Configure the HTTP request pipeline.
            app.MapGrpcService<AdminService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();

        }

        public static async Task StartServer(Server server)
        {
            Console.WriteLine("Server will start accepting connections from the clients");
            await Task.Run(server.Start);
        }
    }
}