using System.Configuration;
using System.Net.Sockets;
using AppServer.Domain;
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
                case (int)Command.ModifyProductData:
                    ExecuteModifyProduct(message);
                    break;
                case (int)Command.ModifyProductImage:
                    ExecuteModifyProductImage(message);
                    break;
            }
        }
        catch (Exception e)
        {
            _dataHandler.SendMessage(command, e.Message);
        }
    }

    private void ExecuteModifyProductImage(string message)
    {
        string[] messageArray = message.Split(Constant.Separator1);
        int productId = Convert.ToInt32(messageArray[0]);
        string username = messageArray[1];
        _productManager.ModifyProductImage(productId, username,_fileCommsHandler);
        _dataHandler.SendMessage((int)Command.ModifyProductImage, "Imagen modificada correctamente");
    }

    private void ExecuteModifyProduct(string message)
    {
        string[] messageArray = message.Split(Constant.Separator1);
        int productId = Convert.ToInt32(messageArray[0]);
        string username = messageArray[1];
        string product = messageArray[2];
        string[] productArray = product.Split(Constant.Separator2);
        string description = productArray[0];
        int stock = Convert.ToInt32(productArray[1]);
        int price = Convert.ToInt32(productArray[2]);

        _productManager.ModifyProductData(productId,username,description,stock,price);

        _dataHandler.SendMessage((int)Command.ModifyProductData, $"{Constant.OkCode}{Constant.Separator1}Producto modificado correctamente");
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
        string[] productArray = review.Split(Constant.Separator1);
        int id = Convert.ToInt32(productArray[0]);
        int rating = Convert.ToInt32(productArray[1]);
        string comment = productArray[2];
        _productManager.AddReview(id,rating,comment);
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
        string responseMessage = $"{Constant.OkCode}{Constant.Separator1}Cliente autenticado correctamente";
        Console.WriteLine("Cliente autenticado correctamente");
        _dataHandler.SendMessage((int)Command.Authenticate, responseMessage);

    }

}
