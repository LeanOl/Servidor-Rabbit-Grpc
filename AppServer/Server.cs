using System.Net;
using System.Net.Sockets;
using System.Text;
using AppServer.Services;
using System.Configuration;
using Protocol;

namespace AppServer;

public class Server
{
   
    static string ipAddress = ConfigurationManager.AppSettings[ServerConfig.serverIPconfigkey];
    static int serverPort = int.Parse(ConfigurationManager.AppSettings[ServerConfig.serverPortconfigkey]);
    static readonly IPEndPoint _endpoint = new IPEndPoint(IPAddress.Parse(ipAddress),serverPort);
    readonly TcpListener _listener = new TcpListener(_endpoint);
    

    public async Task Start()
    {
       _listener.Start();
        Console.WriteLine("Servidor iniciado");
        bool serverOn = true;
        while (serverOn)
        {
            TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Cliente conectado");
            var task=Task.Run(async () => await HandleClientAsync(tcpClient));
        }
    }

    public async Task HandleClientAsync(TcpClient tcpClient)
    {
        bool conectado = true;
        DataHandler dataHandler = new DataHandler(tcpClient);
        while (conectado)
        {
            try
            {
                ServerServices serverServices = new ServerServices(tcpClient);
                (int command, string message) = await dataHandler.ReceiveMessageAsync();
                await serverServices.ExecuteCommandAsync(command,message);
            }
            catch (SocketException e)
            {
                conectado= false;
            }
            
        }
        Console.WriteLine("Cliente desconectado");

    }
}