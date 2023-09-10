using System.Net;
using System.Net.Sockets;
using System.Text;
using Protocol;

namespace AppServer;

public class Server
{
    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),20000);

    public void Start()
    {
        socket.Bind(endpoint);
        socket.Listen(10);
        while (true)
        {
            Socket clientSocket = socket.Accept();
            
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
                byte[] dataLength = dataHandler.Receive(4);
                byte[] data = dataHandler.Receive(BitConverter.ToInt32(dataLength));
                string message = Encoding.UTF8.GetString(data);
            }
            catch (SocketException e)
            {
                conectado= false;
            }
            
        }

    }
}