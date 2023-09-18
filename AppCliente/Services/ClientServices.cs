using System.Net.Sockets;
using System.Text;
using Protocol;
namespace AppClient.Services;

public class ClientServices
{
    private DataHandler _dataHandler;
    private FileCommsHandler _fileCommsHandler;
    private string _username;

    public ClientServices(Socket socket)
    {
       _dataHandler = new DataHandler(socket);
        _fileCommsHandler = new FileCommsHandler(socket);
    }

    public (bool,string) Authenticate(string username, string password)
    {

        SendCredentials(username,password);

        (int responseCommand,string responseMessage)=_dataHandler.ReceiveMessage();
        string[] responseMessageSplit=responseMessage.Split(Protocol.Constant.Separator1);
        int responseCodeInt = Convert.ToInt32(responseMessageSplit[0]);
        responseMessage = responseMessageSplit[1];

        _username=username;

        return (responseCodeInt==1,responseMessage);
    }

    private void SendCredentials(string user,string password)
    {
        string credentials = user + Protocol.Constant.Separator1 + password;
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
            Console.WriteLine("Ingrese la ruta de la imagen");
            string imagePath = Console.ReadLine();
            string product = name + Protocol.Constant.Separator1 + description
                             + Protocol.Constant.Separator1 + stock + Protocol.Constant.Separator1 +
                             price + Protocol.Constant.Separator1 +_username ;
            _dataHandler.SendMessage((int)Command.PublishProduct, product);
            _fileCommsHandler.SendFile(imagePath);
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
       
        foreach (string product in responseMessage.Split(Protocol.Constant.Separator2))
        {
            string[] productArray = product.Split(Protocol.Constant.Separator1);
            Console.WriteLine($"ID: {productArray[0]} Nombre: {productArray[1]} Descripcion: {productArray[2]} Stock: {productArray[3]}");
        }

        Console.WriteLine("Ingrese el ID del producto que desea comprar");

        string id = Console.ReadLine();

        _dataHandler.SendMessage((int)Command.BuyProduct, id);

        (responseCommand, responseMessage) = _dataHandler.ReceiveMessage();

        return responseMessage;
    }
}

