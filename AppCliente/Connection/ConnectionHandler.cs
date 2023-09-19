using System.Net;
using System.Net.Sockets;
using System.Configuration;
namespace AppClient.Connection;

public class ConnectionHandler
{
    private Socket _socketCliente;
    static string ipAddress = ConfigurationManager.AppSettings[ClientConfig.clientIPconfigkey];
    IPEndPoint endpointLocal = new IPEndPoint(IPAddress.Parse(ipAddress),0);
    static string serverIpAddress = ConfigurationManager.AppSettings[ClientConfig.serverIPconfigkey];
    static int serverPort = int.Parse(ConfigurationManager.AppSettings[ClientConfig.serverPortconfigkey]);
    private IPEndPoint endpointRemoto = new IPEndPoint(IPAddress.Parse(serverIpAddress),serverPort);

    public Socket Connect()
    {
        _socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socketCliente.Bind(endpointLocal);
        _socketCliente.Connect(endpointRemoto);
        
        return _socketCliente;
    }

    public void Disconnect()
    {
        _socketCliente.Shutdown(SocketShutdown.Both);
        _socketCliente.Close();
    }


    
}