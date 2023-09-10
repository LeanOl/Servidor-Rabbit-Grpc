using System.Net.Sockets;

namespace AppCliente;
using AppCliente.Conexion;
public class Menu
{
    SocketHandler _socketHandler = new SocketHandler();
    public void StartMenu()
    {
        bool connectionOk = false;
        while (!connectionOk)
        {
            Console.WriteLine("Ingrese su usuario");
            string user = Console.ReadLine();
            Console.WriteLine("Ingrese su contraseña");
            string password = Console.ReadLine();


            try
            {
                _socketHandler.Connect(user, password);
                connectionOk = true;
                MainMenu();
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error al conectarse al servidor");
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error");
                Console.WriteLine(e.Message);
            }
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
            switch (opcion)
            {
                case "1":
                  Console.WriteLine("Publicar producto");
                  break;
              case "2":
                    Console.WriteLine("Comprar Producto"); 
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
                  _socketHandler.Disconnect();
                  salir = true;
                  break;
              default:
                  Console.WriteLine("Opción no valida");
                  break;
            }
        }
        
    }
}