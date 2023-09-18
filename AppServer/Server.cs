using System.Net;
using System.Net.Sockets;
using System.Text;
using AppServer.Services;
using System.Configuration;
using Protocol;

namespace AppServer;

public class Server
{
    readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    static string ipAddress = ConfigurationManager.AppSettings[ServerConfig.serverIPconfigkey];
    static int serverPort = int.Parse(ConfigurationManager.AppSettings[ServerConfig.serverPortconfigkey]);
    readonly IPEndPoint _endpoint = new IPEndPoint(IPAddress.Parse(ipAddress),serverPort);
    

    public void Start()
    {
        _socket.Bind(_endpoint);
        _socket.Listen(10);
        while (true)
        {
            Socket clientSocket = _socket.Accept();
            Console.WriteLine("Cliente conectado");
            new Thread(() => HandleClient(clientSocket)).Start();
        }
    }

    public void HandleClient(Socket clientSocket)
    {
        bool conectado = true;
        DataHandler dataHandler = new DataHandler(clientSocket);
        while (conectado)
        {
            try
            {
                ServerServices serverServices = new ServerServices(clientSocket);
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