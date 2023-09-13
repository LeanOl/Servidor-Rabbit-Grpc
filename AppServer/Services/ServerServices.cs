using System.Text;
using Protocol;

namespace AppServer.Services;

public class ServerServices
{
    public void ExecuteCommand(int command, DataHandler dataHandler)
    {
        switch (command)
        {
            case (int)Command.Authenticate:
                ExecuteAuthentication(dataHandler);
                break;
        }
    }

    private void ExecuteAuthentication(DataHandler dataHandler)
    {
        ClientAuthenticator clientAuthenticator = new ClientAuthenticator();
        Byte[] credentialsLength = dataHandler.Receive(4);
        Byte[] credentials = dataHandler.Receive(BitConverter.ToInt32(credentialsLength));
        
        bool authentication = clientAuthenticator.Authenticate(credentials);

        Byte[] responseCode;
        Byte[] responseMessage;

        if (authentication)
        {
            responseCode= BitConverter.GetBytes(1);
           responseMessage= Encoding.UTF8.GetBytes("Cliente autenticado correctamente");
            Console.WriteLine("Cliente autenticado correctamente");
        }
        else
        {
            responseCode= BitConverter.GetBytes(0);
            responseMessage= Encoding.UTF8.GetBytes("Error: usuario o contraseña incorrectos");
            Console.WriteLine("Error: usuario o contraseña incorrectos");
        }
        dataHandler.Send(responseCode);
        Byte[] responseLength = BitConverter.GetBytes(responseMessage.Length);
        dataHandler.Send(responseLength);
        dataHandler.Send(responseMessage);
    }
}