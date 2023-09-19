namespace Protocol;

public class Constant
{
    public const string Separator1 = ":";
    public const string Separator2 = ";";
    public const string Separator3 = ",";

    public static readonly int FixedDataSize = 4;

    public const string OkCode = "1";
    public const string ErrorCode = "0";

    public const int FixedFileSize = 8;
    public const int MaxPacketSize = 32768; //32KB
    

    public static long CalculateFileParts(long fileSize)
    {
        var fileParts = fileSize / MaxPacketSize;
        return fileParts * MaxPacketSize == fileSize ? fileParts : fileParts + 1;
    }
}