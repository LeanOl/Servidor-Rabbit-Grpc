using System.Text;
using Protocol;

namespace AppServer.Services;

public class ServerServices
{
    private ProductManager _productManager = new ProductManager();
    public void ExecuteCommand(int command, string message, DataHandler dataHandler)
    {
        switch (command)
        {
            case (int)Command.Authenticate:
                ExecuteAuthentication(dataHandler,message);
                break;
            case (int)Command.PublishProduct:
                ExecutePublishProduct(dataHandler,message);
                break;
            case (int)Command.GetProducts:
                ExecuteGetProducts(dataHandler,message);
                break;
            case (int)Command.BuyProduct:
                ExecuteBuyProduct(dataHandler,message);
                break;
        }
    }

    private void ExecuteBuyProduct(DataHandler dataHandler, string message)
    {
        try
        { 
            _productManager.BuyProduct(message);
            dataHandler.SendMessage((int)Command.BuyProduct,"Compra realizada correctamente");
        }
        catch (Exception e)
        {
            dataHandler.SendMessage((int)Command.BuyProduct,e.Message);
        }
        
    }

    private void ExecuteGetProducts(DataHandler dataHandler,string message)
    {
        try
        {
            string products = _productManager.GetProducts(message);
            dataHandler.SendMessage((int)Command.GetProducts, products);
        }
        catch (Exception e)
        {
            dataHandler.SendMessage((int)Command.GetProducts, e.Message);
        }
    }

    private void ExecutePublishProduct(DataHandler dataHandler, string product)
    {
        
        bool publish = _productManager.PublishProduct(product);
        string responseMessage;

        if (publish)
        {
            responseMessage= "Producto publicado correctamente";
            Console.WriteLine("Producto publicado correctamente");
        }
        else
        {
            responseMessage="Error no se pudo publicar el producto";
            Console.WriteLine("Error! no se pudo publicar el producto");
        }

        dataHandler.SendMessage((int)Command.PublishProduct,responseMessage);
    }

    private void ExecuteAuthentication(DataHandler dataHandler,string credentials)
    {
        ClientAuthenticator clientAuthenticator = new ClientAuthenticator();
        
        bool authentication = clientAuthenticator.Authenticate(credentials);

        string responseMessage;

        if (authentication)
        {
            responseMessage= $"1{Constant.Separator1}Cliente autenticado correctamente";
            Console.WriteLine("Cliente autenticado correctamente");
        }
        else
        {
            responseMessage= $"0{Constant.Separator1}Error! usuario o contraseña incorrectos";
            Console.WriteLine("Error! usuario o contraseña incorrectos");
        }
        
        dataHandler.SendMessage((int)Command.Authenticate,responseMessage);
    }
}