using System.Text;
using Protocol;

namespace AppServer.Services;

public class ServerServices
{
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
        }
    }

    private void ExecutePublishProduct(DataHandler dataHandler, string product)
    {
        ProductManager productManager = new ProductManager();
        bool publish = productManager.PublishProduct(product);
        string responseMessage;

        if (publish)
        {
            responseMessage= "Producto publicado correctamente";
            Console.WriteLine("Producto publicado correctamente");
        }
        else
        {
            responseMessage="Error: no se pudo publicar el producto";
            Console.WriteLine("Error: no se pudo publicar el producto");
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
            responseMessage= "1:Cliente autenticado correctamente";
            Console.WriteLine("Cliente autenticado correctamente");
        }
        else
        {
            responseMessage= "0:Error! usuario o contraseña incorrectos";
            Console.WriteLine("Error! usuario o contraseña incorrectos");
        }
        
        dataHandler.SendMessage((int)Command.Authenticate,responseMessage);
    }
}