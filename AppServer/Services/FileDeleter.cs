namespace AppServer.Services;

public static class FileDeleter
{
    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}