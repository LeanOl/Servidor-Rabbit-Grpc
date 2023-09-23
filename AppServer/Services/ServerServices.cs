using System.Configuration;
using System.Net.Sockets;
using Protocol;

namespace AppServer.Services;

public class ServerServices
{
    private ProductManager _productManager = new();
    private DataHandler _dataHandler;
    private FileCommsHandler _fileCommsHandler;

    public ServerServices(Socket socket)
    {
        _dataHandler = new DataHandler(socket);
        _fileCommsHandler = new FileCommsHandler(socket);
    }

    public void ExecuteCommand(int command, string message)
    {
        try
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
                case (int)Command.GetSpecificProduct:
                    ExecuteGetSpecificProduct(message);
                    break;
                case (int)Command.RateProduct:
                    ExecuteRateProduct(message);
                    break;
                case (int)Command.DeleteProduct:
                    ExecuteDeleteProduct(message);
                    break;
            }
        }
        catch (Exception e)
        {
            _dataHandler.SendMessage(command, e.Message);
        }
    }

    private void ExecuteDeleteProduct(string message)
    {
        string[] messageArray = message.Split(Constant.Separator1);
        int productId = Convert.ToInt32(messageArray[0]);
        string username = messageArray[1];
        _productManager.DeleteProduct(productId,username);
        _dataHandler.SendMessage((int)Command.DeleteProduct, "Producto eliminado correctamente");
    }

    private void ExecuteRateProduct(string review)
    {
        _productManager.AddReview(review);
        _dataHandler.SendMessage((int)Command.RateProduct, "Review agregada correctamente");
    }

    private void ExecuteGetSpecificProduct(string productId)
    {
        (string product, string imagePath) = _productManager.GetSpecificProduct(productId);
        _dataHandler.SendMessage((int)Command.GetSpecificProduct, product);
        _fileCommsHandler.SendFile(imagePath);
    }

    private void ExecuteBuyProduct(string message)
    {
        _productManager.BuyProduct(message);
        _dataHandler.SendMessage((int)Command.BuyProduct, "Compra realizada correctamente");
    }

    private void ExecuteGetProducts(string message)
    {
        string products = _productManager.GetProducts(message);
        _dataHandler.SendMessage((int)Command.GetProducts, products);
    }

    private void ExecutePublishProduct(string product)
    {
        
        _productManager.PublishProduct(product,_fileCommsHandler);
        string responseMessage = "Producto publicado correctamente";
        Console.WriteLine(responseMessage);
        _dataHandler.SendMessage((int)Command.PublishProduct, responseMessage);
    }

    private void ExecuteAuthentication(string credentials)
    {
       
        ClientAuthenticator clientAuthenticator = new ClientAuthenticator();
        clientAuthenticator.Authenticate(credentials);
        string responseMessage = $"1{Constant.Separator1}Cliente autenticado correctamente";
        Console.WriteLine("Cliente autenticado correctamente");
        _dataHandler.SendMessage((int)Command.Authenticate, responseMessage);

    }

}
