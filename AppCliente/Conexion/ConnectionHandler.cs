using System.Net;
using System.Net.Sockets;
namespace AppCliente.Conexion;

public class SocketHandler
{
    Socket socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    IPEndPoint endpointLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"),0);
    private IPEndPoint endpointRemoto = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
    public void Connect(string? usuario, string? contraseña)
    {
        socketCliente.Bind(endpointLocal);
        socketCliente.Connect(endpointRemoto);
    }

    public void Disconnect()
    {
        socketCliente.Shutdown(SocketShutdown.Both);
        socketCliente.Close();
    }

    public void Send(Byte[] data)
    {
        int offset = 0;
        int size = data.Length;
        while (offset < size)
        {
            int sent = socketCliente.Send(data, offset, size - offset, SocketFlags.None);
            if (sent == 0)
                throw new SocketException();
            offset += sent;
        }
    }

    public byte[] Receive(int size)
    {
        byte[] data = new byte[size];
        int offset = 0;
        while (offset < size)
        {
            int received = socketCliente.Receive(data, offset, size - offset, SocketFlags.None);
            if (received == 0)
                throw new SocketException();
            offset += received;
        }
        return data;
    }
}