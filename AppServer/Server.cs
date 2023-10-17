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
    

    public void Start()
    {
       _listener.Start();
        Console.WriteLine("Servidor iniciado");
        bool serverOn = true;
        while (serverOn)
        {
            TcpClient tcpClient = _listener.AcceptTcpClient();
            Console.WriteLine("Cliente conectado");
            new Thread(() => HandleClient(tcpClient)).Start();
        }
    }

    public void HandleClient(TcpClient tcpClient)
    {
        bool conectado = true;
        DataHandler dataHandler = new DataHandler(tcpClient);
        while (conectado)
        {
            try
            {
                ServerServices serverServices = new ServerServices(tcpClient);
                (int command, string message) = dataHandler.ReceiveMessage();
                serverServices.ExecuteCommand(command,message);
            }
            catch (SocketException e)
            {
                conectado= false;
            }
            
        }
        Console.WriteLine("Cliente desconectado");

    }
}