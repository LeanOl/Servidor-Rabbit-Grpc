namespace AppClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            bool seguir = true;
            while (seguir)
            {
                Menu menu = new Menu();
                await menu.StartMenuAsync();
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