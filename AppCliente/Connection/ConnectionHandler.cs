using System.Net;
using System.Configuration;
using System.Net.Sockets;

namespace AppClient.Connection;

public class ConnectionHandler
{
    private TcpClient _tcpClient;
    static string ipAddress = ConfigurationManager.AppSettings[ClientConfig.clientIPconfigkey];
    IPEndPoint endpointLocal = new IPEndPoint(IPAddress.Parse(ipAddress),0);
    static string serverIpAddress = ConfigurationManager.AppSettings[ClientConfig.serverIPconfigkey];
    static int serverPort = int.Parse(ConfigurationManager.AppSettings[ClientConfig.serverPortconfigkey]);
    private IPEndPoint endpointRemoto = new IPEndPoint(IPAddress.Parse(serverIpAddress),serverPort);

    public TcpClient Connect()
    {
        _tcpClient = new TcpClient(endpointLocal);
        _tcpClient.Connect(endpointRemoto);

        return _tcpClient;
    }

    public void Disconnect()
    {
        _tcpClient.Close();
    }


    
}