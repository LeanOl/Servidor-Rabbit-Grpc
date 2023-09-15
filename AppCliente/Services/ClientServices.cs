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

        SendCredentials(username,password);

        (int responseCommand,string responseMessage)=_dataHandler.ReceiveMessage();
        string[] responseMessageSplit=responseMessage.Split(":");
        int responseCodeInt = Convert.ToInt32(responseMessageSplit[0]);
        responseMessage = responseMessageSplit[1];

        _username=username;

        return (responseCodeInt==1,responseMessage);
    }

    private void SendCredentials(string user,string password)
    {
        string credentials = user + ":" + password;
        _dataHandler.SendMessage((int)Command.Authenticate, credentials);
        
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

            _dataHandler.SendMessage((int)Command.PublishProduct, product);

            (int responseCommand, string responseMessage) = _dataHandler.ReceiveMessage();

            return responseMessage;

        }
        catch (FormatException ex)
        {
            Console.WriteLine("El valor del stock/precio debe ser un numero");
            return "Error";
        }
        
        

    }

    public string BuyProduct()
    {
        string productName = "";
        _dataHandler.SendMessage((int)Command.GetProducts, productName);

        (int responseCommand, string responseMessage) = _dataHandler.ReceiveMessage();
       
        foreach (var product in responseMessage.Split(":"))
        {
            Console.WriteLine(product);
        }

        Console.WriteLine("Ingrese el ID del producto que desea comprar");

        string id = Console.ReadLine();

        _dataHandler.SendMessage((int)Command.BuyProduct, id);

        (responseCommand, responseMessage) = _dataHandler.ReceiveMessage();

        return responseMessage;
    }
}

