using System.Net.Sockets;
using AppClient.Connection;
using AppClient.Services;

namespace AppClient;

public class Menu
{
    private readonly ConnectionHandler _connectionHandler = new ConnectionHandler();
    private ClientServices? _clientServices;
    public void StartMenu()
    {
        try
        {

            bool authenticated = false;
            while (!authenticated)
            {
                Socket socket = _connectionHandler.Connect();
                _clientServices = new ClientServices(socket);
                Console.WriteLine("Ingrese su usuario");
                string user = Console.ReadLine();
                Console.WriteLine("Ingrese su contraseña");
                string password = Console.ReadLine();

                (authenticated, string responseMessage) = _clientServices.Authenticate(user, password);

                if (authenticated)
                {
                    Console.WriteLine(responseMessage);
                    MainMenu();
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
    public void MainMenu()
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

            string opcion = Console.ReadLine();
            string response;
            switch (opcion)
            {
                case "1":
                  Console.WriteLine("Publicar producto");
                   response=_clientServices.PublishProduct();
                  Console.Clear();
                  Console.WriteLine(response);
                  break;
              case "2":
                  Console.WriteLine("Comprar Producto");
                   response=_clientServices.BuyProduct();
                  Console.Clear();
                  Console.WriteLine(response);
                  break;
              case "3":
                  Console.Clear();
                  Console.WriteLine("Modificar Producto");
                  _clientServices.ModifyProduct();
                  break;
              case "4":
                  Console.Clear();
                  Console.WriteLine("Eliminar producto");
                  response= _clientServices.DeleteProduct();
                  Console.WriteLine(response);
                  break;
              case "5":
                  Console.Clear();
                  Console.WriteLine("Ver productos");
                  response= _clientServices.GetProducts();
                  Console.WriteLine(response);
                  break;
              case "6":
                  Console.Clear();
                  Console.WriteLine("Ver producto especifico");
                  response= _clientServices.GetSpecificProduct();
                  Console.WriteLine(response);
                  break;
              case "7":
                  Console.Clear();
                  Console.WriteLine("Calificar Producto");
                  response= _clientServices.RateProduct();
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
        
    }
}