using System.Net.Sockets;
using AppClient.Connection;
using AppClient.Services;

namespace AppClient;

public class Menu
{
    private readonly ConnectionHandler _connectionHandler = new ConnectionHandler();
    private ClientServices? _clientServices;
    public async Task StartMenuAsync()
    {
        try
        {

            bool authenticated = false;
            while (!authenticated)
            {
                Console.WriteLine("Ingrese su usuario");
                string user = Console.ReadLine();
                Console.WriteLine("Ingrese su contraseña");
                string password = Console.ReadLine();

                TcpClient tcpClient = await _connectionHandler.ConnectAsync();
                _clientServices = new ClientServices(tcpClient);

                (authenticated, string responseMessage) = await _clientServices.AuthenticateAsync(user, password);

                if (authenticated)
                {
                    Console.WriteLine(responseMessage);
                    await MainMenu();
                }
                else
                {
                    Console.WriteLine(responseMessage);
                    _connectionHandler.Disconnect();
                }
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Error al conectarse al servidor");
            Console.WriteLine(ex.Message);
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error inesperado");
            Console.WriteLine(ex.Message);
        }


    }
    public async Task MainMenu()
    {
        bool salir = false;
        while(!salir)
        {
            Console.WriteLine("1- Publicar producto \n" +
                              "2- Comprar Producto \n" +
                              "3- Modificar Producto \n" +
                              "4- Eliminar producto \n" +
                              "5- Ver productos \n" +
                              "6- Ver producto especifico \n" +
                              "7- Calificar Producto \n" +
                              "exit- Desconectarse");
            try
            {
                string opcion = Console.ReadLine();
                string response;
                switch (opcion)
                {
                    case "1":
                        Console.WriteLine("Publicar producto");
                        response = await _clientServices.PublishProductAsync();
                        Console.Clear();
                        Console.WriteLine(response);
                        break;
                    case "2":
                        Console.WriteLine("Comprar Producto");
                        response = await _clientServices.BuyProductAsync();
                        Console.Clear();
                        Console.WriteLine(response);
                        break;
                    case "3":
                        Console.Clear();
                        Console.WriteLine("Modificar Producto");
                        await _clientServices.ModifyProductAsync();
                        break;
                    case "4":
                        Console.Clear();
                        Console.WriteLine("Eliminar producto");
                        response = await _clientServices.DeleteProductAsync();
                        Console.WriteLine(response);
                        break;
                    case "5":
                        Console.Clear();
                        Console.WriteLine("Ver productos");
                        response = await _clientServices.GetProductsAsync();
                        Console.WriteLine(response);
                        break;
                    case "6":
                        Console.Clear();
                        Console.WriteLine("Ver producto especifico");
                        response = await _clientServices.GetSpecificProductAsync();
                        Console.WriteLine(response);
                        break;
                    case "7":
                        Console.Clear();
                        Console.WriteLine("Calificar Producto");
                        response = await _clientServices.RateProductAsync();
                        Console.WriteLine(response);
                        break;
                    case "exit":
                        _connectionHandler.Disconnect();
                        salir = true;
                        break;
                    default:
                        Console.WriteLine("Opción no valida");
                        break;
                }
            }
            catch (IOException e)
            {
                salir = true;
                Console.WriteLine($"Error al conectarse al servidor:{e.Message}");
            }
            catch (InvalidOperationException e)
            {
                salir = true;
                Console.WriteLine($"Error al conectarse al servidor:{e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }   
        }
        
    }
}