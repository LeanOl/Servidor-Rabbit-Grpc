namespace AppClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool seguir = true;
            while (seguir)
            {
                Menu menu = new Menu();
                menu.StartMenu();
                Console.WriteLine("Quieres cerrar la aplicacion y/n");
                string answer = Console.ReadLine();
                if (answer == "y")
                {
                    seguir=false;
                }

            }
           
        }
    }
}