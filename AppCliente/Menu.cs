namespace AppCliente;

public class Menu
{
    public void MenuInicio()
    {
        bool salir = false;
        while(!salir)
        {
            Console.WriteLine("1- Conectarse al servidor \n" +
                              "2- Publicar producto \n" +
                              "3- Comprar Producto \n" +
                              "4- Modificar Producto \n" +
                              "5- Eliminar producto \n" +
                              "6- Ver productos \n" +
                              "7- Ver producto especifico \n" +
                              "8- Calificar Producto \n" +
                              "exit- Salir");

            string opcion = Console.ReadLine();
            switch (opcion)
            {
              case "1":
                Console.WriteLine("Conectarse al servidor");
                break;
              case "2":
                  Console.WriteLine("Publicar producto");
                  break;
              case "3":
                    Console.WriteLine("Comprar Producto"); 
                    break;
              case "4":
                  Console.WriteLine("Modificar Producto");
                  break;
              case "5":
                  Console.WriteLine("Eliminar producto");
                  break;
              case "6":
                  Console.WriteLine("Ver productos");
                  break;
              case "7":
                  Console.WriteLine("Ver producto especifico");
                  break;
              case "8":
                  Console.WriteLine("Calificar Producto"); 
                  break;
              case "exit":
                  salir = true;
                  break;
              default:
                  Console.WriteLine("Opción no valida");
                  break;
            }
        }
        
    }
}