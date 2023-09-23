using System.Configuration;
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
        string[] responseMessageSplit=responseMessage.Split(Constant.Separator1);
        int responseCodeInt = Convert.ToInt32(responseMessageSplit[0]);
        responseMessage = responseMessageSplit[1];

        _username=username;

        return (responseCodeInt==1,responseMessage);
    }

    private void SendCredentials(string user,string password)
    {
        string credentials = user + Constant.Separator1 + password;
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
            string product = name + Constant.Separator1 + description
                             + Constant.Separator1 + stock + Constant.Separator1 +
                             price + Constant.Separator1 +_username ;
            _dataHandler.SendMessage((int)Command.PublishProduct, product);
            _fileCommsHandler.SendFile(imagePath);
            (int responseCommand, string responseMessage) = _dataHandler.ReceiveMessage();

            return responseMessage;

        }
        catch (FormatException ex)
        {
            Console.WriteLine("El valor del stock/precio debe ser un numero");
            return ex.Message;
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Error al enviar el archivo");
            return ex.Message;
        }

    }

    public string BuyProduct()
    {
        try
        {
            string productName = "";
            _dataHandler.SendMessage((int)Command.GetProducts, productName);

            (int responseCommand, string responseMessage) = _dataHandler.ReceiveMessage();

            foreach (string product in responseMessage.Split(Constant.Separator2))
            {
                string[] productArray = product.Split(Constant.Separator1);
                Console.WriteLine(
                    $"ID: {productArray[0]} Nombre: {productArray[1]} Descripcion: {productArray[2]} Stock: {productArray[3]}");
            }

            Console.WriteLine("Ingrese el ID del producto que desea comprar");

            string id = Console.ReadLine();

            _dataHandler.SendMessage((int)Command.BuyProduct, id);

            (responseCommand, responseMessage) = _dataHandler.ReceiveMessage();

            return responseMessage;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string GetProducts()
    {
        try
        {
            Console.WriteLine("Ingrese el nombre del producto");
            string productName = Console.ReadLine();
            _dataHandler.SendMessage((int)Command.GetProducts, productName);
            (int responseCommand, string responseMessage) = _dataHandler.ReceiveMessage();
            foreach (string product in responseMessage.Split(Constant.Separator2))
            {
                string[] productArray = product.Split(Constant.Separator1);
                Console.WriteLine(
                    $"ID: {productArray[0]} Nombre: {productArray[1]} Descripcion: {productArray[2]} Stock: {productArray[3]}");
            }


            return "Consulta exitosa";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public string GetSpecificProduct()
    {
        try
        {
            Console.WriteLine("Ingrese el ID del producto");
            string id = Console.ReadLine();
            _dataHandler.SendMessage((int)Command.GetSpecificProduct, id);
            (int responseCommand, string responseMessage) = _dataHandler.ReceiveMessage();
            string[] productArray = responseMessage.Split(Constant.Separator1);
            if (productArray[0]==Constant.ErrorCode)
                throw new Exception(productArray[1]);
            string imagePath= _fileCommsHandler.ReceiveFile(ConfigurationManager.AppSettings[ClientConfig.clientImagePath]);
            Console.WriteLine(
                               $"ID: {productArray[1]} Nombre: {productArray[2]} \n" +
                               $"Descripcion: {productArray[3]} Stock: {productArray[4]} \n" +
                               $"Precio: {productArray[5]} Imagen: {imagePath} \n" +
                               $"Usuario: {productArray[6]} \n"+
                               $"Calificaciones: {GetStringReviews(productArray[7])}");
            return "Consulta exitosa";
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private string GetStringReviews(string reviews)
    {
        string reviewsString = "";
        foreach (string review in reviews.Split(Constant.Separator2))
        {
            string[] reviewArray = review.Split(Constant.Separator3);
            if (reviewArray.Length == 1)
                break;
            reviewsString += $"Calificacion: {reviewArray[0]} Comentario: {reviewArray[1]} \n";
        }
        return reviewsString;
    }

    public string RateProduct()
    {
        try
        {
            Console.WriteLine("Ingrese el ID del producto");
            string id = Console.ReadLine();
            Console.WriteLine("Ingrese una calificacion del 1 al 10");
            string rating = Console.ReadLine();
            Console.WriteLine("Ingrese el comentario del producto");
            string comment = Console.ReadLine();
            string review = id + Constant.Separator1 + rating + Constant.Separator1 + comment;
            _dataHandler.SendMessage((int)Command.RateProduct, review);
            (int responseCommand, string responseMessage) = _dataHandler.ReceiveMessage();
            return responseMessage;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public string DeleteProduct()
    {
        try
        {
            Console.WriteLine("Ingrese el ID del producto");
            string id = Console.ReadLine();
            string username = _username;
            string message = id + Constant.Separator1 + username;
            _dataHandler.SendMessage((int)Command.DeleteProduct, message);
            (int responseCommand, string responseMessage) = _dataHandler.ReceiveMessage();
            return responseMessage;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}

