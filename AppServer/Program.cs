namespace AppServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
           Server server = new Server();
           server.Start();
        }
    }
}