using System.Net;
using System.Net.Sockets;
using System.Text;
using AppServer.Services;
using Protocol;

namespace AppServer;

public class Server
{
    readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    readonly IPEndPoint _endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),20000);
    

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