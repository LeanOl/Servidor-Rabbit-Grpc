using System.Text;

namespace AppServer.Services;

public class ClientAuthenticator
{
    public void Authenticate(string credentials)
    {
        string[] credentialsArray = credentials.Split(":");
        string username = credentialsArray[0];
        string password = credentialsArray[1];
        if (!UserManager.ValidateUser(username, password))
            throw new Exception("Error! usuario o contraseña incorrectos");
        
            
    }
}