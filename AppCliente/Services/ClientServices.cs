using System.Net.Sockets;
using System.Text;
using Protocol;
namespace AppClient.Services;

public class ClientServices
{
    private DataHandler _dataHandler;

    public ClientServices(Socket socket)
    {
       _dataHandler = new DataHandler(socket);
    }

    public bool Authenticate(string username, string password)
    {
        Byte[] command= new byte[4];
        command= BitConverter.GetBytes((int)Command.Authenticate);
        _dataHandler.Send(command);

        SendCredentials(username,password);

        Byte[] response= _dataHandler.Receive(4);
        int responseCode = BitConverter.ToInt32(response);
        
        return responseCode == 1;
    }

    private void SendCredentials(string user,string password)
    {
        string credentials = user + ":" + password;

        
        Byte[] size = BitConverter.GetBytes(credentials.Length);
        _dataHandler.Send(size);

        
        Byte[] message = Encoding.UTF8.GetBytes(credentials);
        _dataHandler.Send(message);
        
    }
}

