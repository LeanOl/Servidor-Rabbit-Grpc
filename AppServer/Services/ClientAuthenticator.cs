using Protocol;

namespace AppServer.Services;

public class ClientAuthenticator
{
    public void Authenticate(string credentials)
    {
        string[] credentialsArray = credentials.Split(Constant.Separator1);
        string username = credentialsArray[0];
        string password = credentialsArray[1];
        if (!UserManager.ValidateUser(username, password))
            throw new Exception($"{Constant.ErrorCode}{Constant.Separator1}Error! usuario o contraseña incorrectos");
        
            
    }
}