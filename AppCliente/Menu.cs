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
            Socket socket =_connectionHandler.Connect();
           _clientServices = new ClientServices(socket);

           bool authenticated = false;
           while (!authenticated)
           {
               Console.WriteLine("Ingrese su usuario");
               string user = Console.ReadLine();
               Console.WriteLine("Ingrese su contraseña");
               string password = Console.ReadLine();
               
               (authenticated,string responseMessage)=_clientServices.Authenticate(user, password);

              if (authenticated)
              {
                  Console.WriteLine(responseMessage);
                  MainMenu();
               }else{
                  Console.WriteLine(responseMessage);
              }
           }
        }
        catch(SocketException ex) 
        {
            Console.WriteLine("Error al conectarse al servidor");
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
                  Console.WriteLine("Modificar Producto");
                  break;
              case "4":
                  Console.WriteLine("Eliminar producto");
                  break;
              case "5":
                  Console.WriteLine("Ver productos");
                  break;
              case "6":
                  Console.WriteLine("Ver producto especifico");
                  break;
              case "7":
                  Console.WriteLine("Calificar Producto"); 
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