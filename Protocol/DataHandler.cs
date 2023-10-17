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

        public (int, string) ReceiveMessage()
        {
            Byte[] command= Receive(4);
            int commandInt= BitConverter.ToInt32(command);
            Byte[] length= Receive(4);
            Byte[] message= Receive(BitConverter.ToInt32(length));
            string messageString = Encoding.UTF8.GetString(message);
            return (commandInt,messageString);
        }

        public void SendMessage(int command, string message)
        {
            Byte[] commandBytes= BitConverter.GetBytes(command);
            Send(commandBytes);
            Byte[] lengthBytes= BitConverter.GetBytes(message.Length);
            Send(lengthBytes);
            Byte[] messageBytes= Encoding.UTF8.GetBytes(message);
            Send(messageBytes);
        }
        public void Send(Byte[] data)
        {
            try
            {
                NetworkStream stream = _tcpClient.GetStream();
                stream.Write(data);
            }
            catch (SocketException)
            {
                throw new SocketException();
            }
        }

        public byte[] Receive(int size)
        {
            byte[] data = new byte[size];
            int offset = 0;
            NetworkStream stream = _tcpClient.GetStream();
            while (offset < size)
            {
                int received = stream.Read(data, offset, size - offset);
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