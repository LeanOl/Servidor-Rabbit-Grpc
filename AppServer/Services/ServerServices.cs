using System.Configuration;
using System.Net.Sockets;
using Protocol;

namespace AppServer.Services;

public class ServerServices
{
    private ProductManager _productManager = new();
    private DataHandler _dataHandler;
    private FileCommsHandler _fileCommsHandler;

    public ServerServices(TcpClient tcpClient)
    {
        _dataHandler = new DataHandler(tcpClient);
        _fileCommsHandler = new FileCommsHandler(tcpClient);
    }

    public async Task ExecuteCommandAsync(int command, string message)
    {
        try
        {
            
            switch (command)
            {
                case (int)Command.Authenticate:
                    await ExecuteAuthenticationAsync(message);
                    break;
                case (int)Command.PublishProduct:
                    await ExecutePublishProductAsync(message);
                    break;
                case (int)Command.GetProducts:
                    await ExecuteGetProductsAsync(message);
                    break;
                case (int)Command.BuyProduct:
                    await ExecuteBuyProductAsync(message);
                    break;
                case (int)Command.GetSpecificProduct:
                    await ExecuteGetSpecificProductAsync(message);
                    break;
                case (int)Command.RateProduct:
                    await ExecuteRateProductAsync(message);
                    break;
                case (int)Command.DeleteProduct:
                    await ExecuteDeleteProductAsync(message);
                    break;
                case (int)Command.ModifyProductData:
                    await ExecuteModifyProductAsync(message);
                    break;
                case (int)Command.ModifyProductImage:
                    await ExecuteModifyProductImageAsync(message);
                    break;
            }
        }
        catch (Exception e)
        {
            await _dataHandler.SendMessageAsync(command, e.Message);
        }
    }

    private async Task ExecuteModifyProductImageAsync(string message)
    {
        string[] messageArray = message.Split(Constant.Separator1);
        int productId = Convert.ToInt32(messageArray[0]);
        string username = messageArray[1];
        await _productManager.ModifyProductImage(productId, username,_fileCommsHandler);
        await _dataHandler.SendMessageAsync((int)Command.ModifyProductImage, "Imagen modificada correctamente");
    }

    private async Task ExecuteModifyProductAsync(string message)
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
        await _dataHandler.SendMessageAsync((int)Command.ModifyProductData, $"{Constant.OkCode}{Constant.Separator1}Producto modificado correctamente");
    }

    private async Task ExecuteDeleteProductAsync(string message)
    {
        string[] messageArray = message.Split(Constant.Separator1);
        int productId = Convert.ToInt32(messageArray[0]);
        string username = messageArray[1];
        _productManager.DeleteProduct(productId,username);
        await _dataHandler.SendMessageAsync((int)Command.DeleteProduct, "Producto eliminado correctamente");
    }

    private async Task ExecuteRateProductAsync(string review)
    {
        _productManager.AddReview(review);
        await _dataHandler.SendMessageAsync((int)Command.RateProduct, "Review agregada correctamente");
    }

    private async Task ExecuteGetSpecificProductAsync(string productId)
    {
        (string product, string imagePath) = _productManager.GetSpecificProduct(productId);
        await _dataHandler.SendMessageAsync((int)Command.GetSpecificProduct, product);
        await _fileCommsHandler.SendFileAsync(imagePath);
    }

    private async Task ExecuteBuyProductAsync(string message)
    {
        _productManager.BuyProduct(message);
        await _dataHandler.SendMessageAsync((int)Command.BuyProduct, "Compra realizada correctamente");
    }

    private async Task ExecuteGetProductsAsync(string message)
    {
        string products = _productManager.GetProducts(message);
        await _dataHandler.SendMessageAsync((int)Command.GetProducts, products);
    }

    private async Task ExecutePublishProductAsync(string product)
    {
        await _productManager.PublishProductAsync(product,_fileCommsHandler);
        string responseMessage = "Producto publicado correctamente";
        Console.WriteLine(responseMessage);
        await _dataHandler.SendMessageAsync((int)Command.PublishProduct, responseMessage);
    }

    private async Task ExecuteAuthenticationAsync(string credentials)
    {
       
        ClientAuthenticator clientAuthenticator = new ClientAuthenticator();
        clientAuthenticator.Authenticate(credentials);
        string responseMessage = $"{Constant.OkCode}{Constant.Separator1}Cliente autenticado correctamente";
        Console.WriteLine("Cliente autenticado correctamente");
        await _dataHandler.SendMessageAsync((int)Command.Authenticate, responseMessage);

    }

}
