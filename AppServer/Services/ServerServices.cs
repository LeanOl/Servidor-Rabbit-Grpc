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
        
        int authentication = clientAuthenticator.Authenticate(credentials);

        dataHandler.Send(BitConverter.GetBytes(authentication));
        if (authentication == 1)
        {
            Console.WriteLine("Client authenticated");
        }
        else
        {
            Console.WriteLine("Client failed authentication");
        }
    }
}