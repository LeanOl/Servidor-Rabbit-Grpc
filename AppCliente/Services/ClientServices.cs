using System.Net.Sockets;
using System.Text;
using Protocol;
namespace AppClient.Services;

public class ClientServices
{
    private DataHandler _dataHandler;
    private string _username;

    public ClientServices(Socket socket)
    {
       _dataHandler = new DataHandler(socket);
    }

    public (bool,string) Authenticate(string username, string password)
    {
        Byte[] command= new byte[4];
        command= BitConverter.GetBytes((int)Command.Authenticate);
        _dataHandler.Send(command);

        SendCredentials(username,password);

        Byte[] responseCode= _dataHandler.Receive(4);
        int responseCodeInt= BitConverter.ToInt32(responseCode);
        Byte[] responseLength= _dataHandler.Receive(4);
        Byte[] response= _dataHandler.Receive(BitConverter.ToInt32(responseLength));
        string responseMessage = Encoding.UTF8.GetString(response);

        _username=username;

        return (responseCodeInt==1,responseMessage);
    }

    private void SendCredentials(string user,string password)
    {
        string credentials = user + ":" + password;

        
        Byte[] size = BitConverter.GetBytes(credentials.Length);
        _dataHandler.Send(size);

        
        Byte[] message = Encoding.UTF8.GetBytes(credentials);
        _dataHandler.Send(message);
        
    }

    public string PublishProduct()
    {
        try
        {
            Console.WriteLine("Ingrese el nombre del producto");
            string name = Console.ReadLine();
            Console.WriteLine("Ingrese descripcion del producto");
            string description = Console.ReadLine();
            Console.WriteLine("Ingrese el stock del producto");
            int stock = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Ingrese el precio del producto");
            int price = Convert.ToInt32(Console.ReadLine());
            string product = name + ":" + description + ":" + stock + ":" + price + ":"+_username ;

            Byte[] command = BitConverter.GetBytes((int)Command.PublishProduct);
            _dataHandler.Send(command);
            Byte[] size = BitConverter.GetBytes(product.Length);
            _dataHandler.Send(size);
            Byte[] message = Encoding.UTF8.GetBytes(product);
            _dataHandler.Send(message);

            Byte[] responseCode = _dataHandler.Receive(4);
            int responseCodeInt = BitConverter.ToInt32(responseCode);
            Byte[] responseLength = _dataHandler.Receive(4);
            Byte[] response = _dataHandler.Receive(BitConverter.ToInt32(responseLength));
            string responseMessage = Encoding.UTF8.GetString(response);

            return responseMessage;

        }
        catch (FormatException ex)
        {
            Console.WriteLine("El valor del stock/precio debe ser un numero");
            return "Error";
        }
        
        

    }
}

