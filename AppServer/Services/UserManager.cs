using System.Collections.Generic;
using System.Configuration;

namespace AppServer.Services;
public class UserManager
{
    private static readonly Dictionary<string, string> users = new Dictionary<string, string>();

    static UserManager()
    {
        var config = ConfigurationManager.AppSettings;

        foreach (var key in config.AllKeys)
        {
            if (key.StartsWith("user_"))
            {
                var username = key.Substring(5);
                users.Add(username, config[key]);
            }
        }
    }

    public static bool ValidateUser(string username, string password)
    {
        if (!users.ContainsKey(username))
        {
            return false;
        }

        return users[username] == password;
    }

}
