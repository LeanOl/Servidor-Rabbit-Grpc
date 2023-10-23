using System.Net.Sockets;

namespace Protocol;

public class FileCommsHandler
{
    private readonly ConversionHandler _conversionHandler;
    private readonly FileHandler _fileHandler;
    private readonly FileStreamHandler _fileStreamHandler;
    private readonly DataHandler _tcpHelper;

    public FileCommsHandler(TcpClient tcpClient)
    {
        _conversionHandler = new ConversionHandler();
        _fileHandler = new FileHandler();
        _fileStreamHandler = new FileStreamHandler();
        _tcpHelper = new DataHandler(tcpClient);
    }

    public async Task SendFileAsync(string path)
    {
        if (_fileHandler.FileExists(path))
        {
            var fileName = _fileHandler.GetFileName(path);
            // ---> Enviar el largo del nombre del archivo
            await _tcpHelper.SendAsync(_conversionHandler.ConvertIntToBytes(fileName.Length));
            // ---> Enviar el nombre del archivo
            await _tcpHelper.SendAsync(_conversionHandler.ConvertStringToBytes(fileName));

            // ---> Obtener el tamaño del archivo
            long fileSize = _fileHandler.GetFileSize(path);
            // ---> Enviar el tamaño del archivo
            var convertedFileSize = _conversionHandler.ConvertLongToBytes(fileSize);
            await _tcpHelper.SendAsync(convertedFileSize);
            // ---> Enviar el archivo (pero con file stream)
            await SendFileWithStreamAsync(fileSize, path);
        }
        else
        {
            throw new FileNotFoundException("File does not exist");
        }
    }
    public async Task SendFileAsync(string path,string fileName)
    {
        if (_fileHandler.FileExists(path))
        {
            // ---> Enviar el largo del nombre del archivo
            await _tcpHelper.SendAsync(_conversionHandler.ConvertIntToBytes(fileName.Length));
            // ---> Enviar el nombre del archivo
            await _tcpHelper.SendAsync(_conversionHandler.ConvertStringToBytes(fileName));

            // ---> Obtener el tamaño del archivo
            long fileSize = _fileHandler.GetFileSize(path);
            // ---> Enviar el tamaño del archivo
            var convertedFileSize = _conversionHandler.ConvertLongToBytes(fileSize);
            await _tcpHelper.SendAsync(convertedFileSize);
            // ---> Enviar el archivo (pero con file stream)
            await SendFileWithStreamAsync(fileSize, path);
        }
        else
        {
            throw new FileNotFoundException("File does not exist");
        }
    }
    public async Task<string> ReceiveFileAsync(string path)
    {
        // ---> Recibir el largo del nombre del archivo
        int fileNameSize = _conversionHandler.ConvertBytesToInt(
            await _tcpHelper.ReceiveAsync(Constant.FixedDataSize));
        // ---> Recibir el nombre del archivo
        string fileName = _conversionHandler.ConvertBytesToString(await _tcpHelper.ReceiveAsync(fileNameSize));
        string fullPath = $@"{path}\{fileName}";
        // ---> Recibir el largo del archivo
        long fileSize = _conversionHandler.ConvertBytesToLong(await _tcpHelper.ReceiveAsync(Constant.FixedFileSize));
        // ---> Recibir el archivo
        ReceiveFileWithStreamsAsync(fileSize, fullPath);

        return fullPath;
    }
    public async Task<string> ReceiveFileAsync(string path,string fileName)
    {
        // ---> Recibir el largo del nombre del archivo
        int fileNameSize = _conversionHandler.ConvertBytesToInt(
           await _tcpHelper.ReceiveAsync(Constant.FixedDataSize));
        // ---> Recibir el nombre del archivo
        string reveivedFileName = _conversionHandler.ConvertBytesToString(
            await _tcpHelper.ReceiveAsync(fileNameSize));
        string fullPath = $@"{path}\{fileName}";
        // ---> Recibir el largo del archivo
        long fileSize = _conversionHandler.ConvertBytesToLong(
            await _tcpHelper.ReceiveAsync(Constant.FixedFileSize));
        // ---> Recibir el archivo
        ReceiveFileWithStreamsAsync(fileSize, fullPath);

        return fullPath;
    }
    private async Task SendFileWithStreamAsync(long fileSize, string path)
    {
        long fileParts = Constant.CalculateFileParts(fileSize);
        long offset = 0;
        long currentPart = 1;

        //Mientras tengo un segmento a enviar
        while (fileSize > offset)
        {
            byte[] data;
            //Es el último segmento?
            if (currentPart == fileParts)
            {
                var lastPartSize = (int)(fileSize - offset);
                //1- Leo de disco el último segmento
                //2- Guardo el último segmento en un buffer
                data = _fileStreamHandler.Read(path, offset, lastPartSize); //Puntos 1 y 2
                offset += lastPartSize;
            }
            else
            {
                //1- Leo de disco el segmento
                //2- Guardo ese segmento en un buffer
                data = _fileStreamHandler.Read(path, offset,Constant.MaxPacketSize);
                offset += Constant.MaxPacketSize;
            }

            await _tcpHelper.SendAsync(data); //3- Envío ese segmento a travez de la red
            currentPart++;
        }
    }

    private async Task ReceiveFileWithStreamsAsync(long fileSize, string fileName)
    {
        long fileParts = Constant.CalculateFileParts(fileSize);
        long offset = 0;
        long currentPart = 1;

        //Mientras tengo partes para recibir
        while (fileSize > offset)
        {
            byte[] data;
            //1- Me fijo si es la ultima parte
            if (currentPart == fileParts)
            {
                //1.1 - Si es, recibo la ultima parte
                var lastPartSize = (int)(fileSize - offset);
                data = await _tcpHelper.ReceiveAsync(lastPartSize);
                offset += lastPartSize;
            }
            else
            {
                //2.2- Si no, recibo una parte cualquiera
                data = await _tcpHelper.ReceiveAsync(Constant.MaxPacketSize);
                offset += Constant.MaxPacketSize;
            }
            //3- Escribo esa parte del archivo a disco
            _fileStreamHandler.Write(fileName, data);
            currentPart++;
        }
    }
}