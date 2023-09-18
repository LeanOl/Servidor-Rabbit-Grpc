using System.Net;
using System.Net.Sockets;
using System.Configuration;
namespace AppClient.Connection;

public class ConnectionHandler
{
    Socket socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    static string ipAddress = ConfigurationManager.AppSettings[ClientConfig.clientIPconfigkey];
    IPEndPoint endpointLocal = new IPEndPoint(IPAddress.Parse(ipAddress),0);
    static string serverIpAddress = ConfigurationManager.AppSettings[ClientConfig.serverIPconfigkey];
    static int serverPort = int.Parse(ConfigurationManager.AppSettings[ClientConfig.serverPortconfigkey]);
    private IPEndPoint endpointRemoto = new IPEndPoint(IPAddress.Parse(serverIpAddress),serverPort);
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