using System.Text;

namespace AppServer.Services;

public class ClientAuthenticator
{
    public int Authenticate(Byte[] credentials)
    {
        string credentialsString = Encoding.UTF8.GetString(credentials);
        string[] credentialsArray = credentialsString.Split(":");
        string username = credentialsArray[0];
        string password = credentialsArray[1];
        return username == "admin" && password == "admin" ? 1 : 0;
    }
}