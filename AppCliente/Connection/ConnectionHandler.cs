using System.Net;
using System.Net.Sockets;

namespace AppClient.Connection;

public class ConnectionHandler
{
    Socket socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    IPEndPoint endpointLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"),0);
    private IPEndPoint endpointRemoto = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
    public Socket Connect()
    {
        socketCliente.Bind(endpointLocal);
        socketCliente.Connect(endpointRemoto);
        
        return socketCliente;
    }

    public void Disconnect()
    {
        socketCliente.Shutdown(SocketShutdown.Both);
        socketCliente.Close();
    }

    
}