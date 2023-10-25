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

    public ClientServices(TcpClient tcpClient)
    {
       _dataHandler = new DataHandler(tcpClient);
        _fileCommsHandler = new FileCommsHandler(tcpClient);
    }

    public async Task<(bool,string)> AuthenticateAsync(string username, string password)
    {

        await SendCredentialsAsync(username,password);

        (int responseCommand,string responseMessage)=await _dataHandler.ReceiveMessageAsync();
        string[] responseMessageSplit=responseMessage.Split(Constant.Separator1);
        int responseCodeInt = Convert.ToInt32(responseMessageSplit[0]);
        responseMessage = responseMessageSplit[1];

        _username=username;

        return (responseCodeInt==1,responseMessage);
    }

    private async Task SendCredentialsAsync(string user,string password)
    {
        string credentials = user + Constant.Separator1 + password;
        await _dataHandler.SendMessageAsync((int)Command.Authenticate, credentials);
        
    }

    public async Task<string> PublishProductAsync()
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
            await _dataHandler.SendMessageAsync((int)Command.PublishProduct, product);
            await _fileCommsHandler.SendFileAsync(imagePath);
            (int responseCommand, string responseMessage) = await _dataHandler.ReceiveMessageAsync();

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

    public async Task<string> BuyProductAsync()
    {
        
            string productName = "";
            await _dataHandler.SendMessageAsync((int)Command.GetProducts, productName);

            (int responseCommand, string responseMessage) = await _dataHandler.ReceiveMessageAsync();

            foreach (string product in responseMessage.Split(Constant.Separator2))
            {
                string[] productArray = product.Split(Constant.Separator1);
                Console.WriteLine(
                    $"ID: {productArray[0]} Nombre: {productArray[1]} Descripcion: {productArray[2]} Stock: {productArray[3]}");
            }

            Console.WriteLine("Ingrese el ID del producto que desea comprar");

            string id = Console.ReadLine();

            await _dataHandler.SendMessageAsync((int)Command.BuyProduct, id);

            (responseCommand, responseMessage) = await _dataHandler.ReceiveMessageAsync();

            return responseMessage;
        
       
    }

    public async Task<string> GetProductsAsync()
    {
        
            Console.WriteLine("Ingrese el nombre del producto");
            string productName = Console.ReadLine();
            await _dataHandler.SendMessageAsync((int)Command.GetProducts, productName);
            (int responseCommand, string responseMessage) = await _dataHandler.ReceiveMessageAsync();
            foreach (string product in responseMessage.Split(Constant.Separator2))
            {
                string[] productArray = product.Split(Constant.Separator1);
                Console.WriteLine(
                    $"ID: {productArray[0]} Nombre: {productArray[1]} Descripcion: {productArray[2]} Stock: {productArray[3]}");
            }


            return "Consulta exitosa";
        
    }

    public async Task<string> GetSpecificProductAsync()
    {
        
            Console.WriteLine("Ingrese el ID del producto");
            string id = Console.ReadLine();
            await _dataHandler.SendMessageAsync((int)Command.GetSpecificProduct, id);
            (int responseCommand, string responseMessage) = await _dataHandler.ReceiveMessageAsync();
            string[] productArray = responseMessage.Split(Constant.Separator1);
            if (productArray[0]==Constant.ErrorCode)
                throw new Exception(productArray[1]);
            string imagePath= await _fileCommsHandler.ReceiveFileAsync(ConfigurationManager.AppSettings[ClientConfig.clientImagePath]);
            Console.WriteLine(
                               $"ID: {productArray[1]} Nombre: {productArray[2]} \n" +
                               $"Descripcion: {productArray[3]} Stock: {productArray[4]} \n" +
                               $"Precio: {productArray[5]} Imagen: {imagePath} \n" +
                               $"Usuario: {productArray[6]} \n"+
                               $"Calificaciones: {GetStringReviews(productArray[7])}");
            return "Consulta exitosa";
        
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

    public async Task<string> RateProductAsync()
    {
        
            Console.WriteLine("Ingrese el ID del producto");
            string id = Console.ReadLine();
            Console.WriteLine("Ingrese una calificacion del 1 al 10");
            string rating = Console.ReadLine();
            Console.WriteLine("Ingrese el comentario del producto");
            string comment = Console.ReadLine();
            string review = id + Constant.Separator1 + rating + Constant.Separator1 + comment;
            await _dataHandler.SendMessageAsync((int)Command.RateProduct, review);
            (int responseCommand, string responseMessage) = await _dataHandler.ReceiveMessageAsync();
            return responseMessage;
        
    }

    public async Task<string> DeleteProductAsync()
    {
        
            Console.WriteLine("Ingrese el ID del producto");
            string id = Console.ReadLine();
            string username = _username;
            string message = id + Constant.Separator1 + username;
            await _dataHandler.SendMessageAsync((int)Command.DeleteProduct, message);
            (int responseCommand, string responseMessage) = await _dataHandler.ReceiveMessageAsync();
            return responseMessage;
        
    }

    public async Task ModifyProductAsync()
    {
        
            Console.WriteLine("Ingrese el ID del producto");
            string id = Console.ReadLine();
            Console.WriteLine("Ingrese la nueva descripcion del producto");
            string description = Console.ReadLine();
            Console.WriteLine("Ingrese el nuevo stock del producto");
            int stock = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Ingrese el nuevo precio del producto");
            int price = Convert.ToInt32(Console.ReadLine());
            string productData = id + Constant.Separator1+ _username+ Constant.Separator1 + description
                             + Constant.Separator2 + stock + Constant.Separator2 +
                             price;
            await _dataHandler.SendMessageAsync((int)Command.ModifyProductData, productData);
            (int responseCommand1, string responseMessage1) = await _dataHandler.ReceiveMessageAsync();
            Console.WriteLine(responseMessage1.Split(Constant.Separator1)[1]);
            string responseCode = responseMessage1.Split(Constant.Separator1)[0];
            if (responseCode == Constant.ErrorCode)
                return;

            Console.WriteLine("Ingrese la nueva imagen del producto");
            string imagePath = Console.ReadLine();
            if (imagePath != "")
            {
                await _dataHandler.SendMessageAsync((int)Command.ModifyProductImage, id+Constant.Separator1+_username);
                await _fileCommsHandler.SendFileAsync(imagePath);
                (int responseCommand, string responseMessage) = await _dataHandler.ReceiveMessageAsync();
                Console.WriteLine(responseMessage);
            }
        }
}

