using System.Net.Sockets;
using System.Text;

namespace Protocol
{
    public class DataHandler
    {
        private readonly Socket _socket;
        public DataHandler(Socket socket)
        {
            _socket= socket;
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
            int offset = 0;
            int size = data.Length;
            while (offset < size)
            {
                int sent = _socket.Send(data, offset, size - offset, SocketFlags.None);
                if (sent == 0)
                    throw new SocketException();
                offset += sent;
            }
        }

        public byte[] Receive(int size)
        {
            byte[] data = new byte[size];
            int offset = 0;
            while (offset < size)
            {
                int received = _socket.Receive(data, offset, size - offset, SocketFlags.None);
                if (received == 0)
                    throw new SocketException();
                offset += received;
            }
            return data;
        }
    }
}