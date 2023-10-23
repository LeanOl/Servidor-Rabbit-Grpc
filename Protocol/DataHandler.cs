using System.Net.Sockets;
using System.Text;

namespace Protocol
{
    public class DataHandler
    {
        private readonly TcpClient _tcpClient;
        public DataHandler(TcpClient tcpClient)
        {
            _tcpClient= tcpClient;
        }

        public async Task<(int, string)> ReceiveMessageAsync()
        {
            Byte[] command= await ReceiveAsync(4);
            int commandInt= BitConverter.ToInt32(command);
            Byte[] length= await ReceiveAsync(4);
            Byte[] message= await ReceiveAsync(BitConverter.ToInt32(length));
            string messageString = Encoding.UTF8.GetString(message);
            return (commandInt,messageString);
        }

        public async Task SendMessageAsync(int command, string message)
        {
            Byte[] commandBytes= BitConverter.GetBytes(command);
            await SendAsync(commandBytes);
            Byte[] lengthBytes= BitConverter.GetBytes(message.Length);
            await SendAsync(lengthBytes);
            Byte[] messageBytes= Encoding.UTF8.GetBytes(message);
            await SendAsync(messageBytes);
        }
        public async Task SendAsync(Byte[] data)
        {
            try
            {
                NetworkStream stream = _tcpClient.GetStream();
                await stream.WriteAsync(data);
            }
            catch (SocketException)
            {
                throw new SocketException();
            }
        }

        public async Task<byte[]> ReceiveAsync(int size)
        {
            byte[] data = new byte[size];
            int offset = 0;
            NetworkStream stream = _tcpClient.GetStream();
            while (offset < size)
            {
                int received = await stream.ReadAsync(data, offset, size - offset);
                if (received == 0)
                {
                    throw new SocketException();
                }
                offset += received;
            }
            return data;
        }
    }
}