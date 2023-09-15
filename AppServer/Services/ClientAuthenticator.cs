using System.Text;

namespace AppServer.Services;

public class ClientAuthenticator
{
    public bool Authenticate(string credentials)
    {
        string[] credentialsArray = credentials.Split(":");
        string username = credentialsArray[0];
        string password = credentialsArray[1];
        return username == "admin" && password == "admin" ;
    }
}