using System.Net.Sockets;
using System.Text;
using Protocol;

namespace AppServer.Services;

public class ServerServices
{
    private ProductManager _productManager = new ProductManager();
    private DataHandler _dataHandler;
    private FileCommsHandler _fileCommsHandler;

    public ServerServices(Socket socket)
    {
        _dataHandler = new DataHandler(socket);
        _fileCommsHandler = new FileCommsHandler(socket);
    }
    public void ExecuteCommand(int command, string message)
    {
        switch (command)
        {
            case (int)Command.Authenticate:
                ExecuteAuthentication(message);
                break;
            case (int)Command.PublishProduct:
                ExecutePublishProduct(message);
                break;
            case (int)Command.GetProducts:
                ExecuteGetProducts(message);
                break;
            case (int)Command.BuyProduct:
                ExecuteBuyProduct(message);
                break;
        }
    }

    private void ExecuteBuyProduct( string message)
    {
        try
        { 
            _productManager.BuyProduct(message);
            _dataHandler.SendMessage((int)Command.BuyProduct,"Compra realizada correctamente");
        }
        catch (Exception e)
        {
            _dataHandler.SendMessage((int)Command.BuyProduct,e.Message);
        }
        
    }

    private void ExecuteGetProducts(string message)
    {
        try
        {
            string products = _productManager.GetProducts(message);
            _dataHandler.SendMessage((int)Command.GetProducts, products);
        }
        catch (Exception e)
        {
            _dataHandler.SendMessage((int)Command.GetProducts, e.Message);
        }
    }

    private void ExecutePublishProduct(string product)
    {
        try
        {
            string imagePath = _fileCommsHandler.ReceiveFile();
            _productManager.PublishProduct(product,imagePath);
            string responseMessage;

            responseMessage = "Producto publicado correctamente";
            Console.WriteLine("Producto publicado correctamente");
            
            _dataHandler.SendMessage((int)Command.PublishProduct, responseMessage);
        }
        catch (Exception e)
        {
            _dataHandler.SendMessage((int)Command.PublishProduct, e.Message);
        }
        
    }

    private void ExecuteAuthentication(string credentials)
    {
        ClientAuthenticator clientAuthenticator = new ClientAuthenticator();
        
        bool authentication = clientAuthenticator.Authenticate(credentials);

        string responseMessage;

        if (authentication)
        {
            responseMessage= $"1{Protocol.Constant.Separator1}Cliente autenticado correctamente";
            Console.WriteLine("Cliente autenticado correctamente");
        }
        else
        {
            responseMessage= $"0{Protocol.Constant.Separator1}Error! usuario o contraseña incorrectos";
            Console.WriteLine("Error! usuario o contraseña incorrectos");
        }
        
        _dataHandler.SendMessage((int)Command.Authenticate,responseMessage);
    }
}