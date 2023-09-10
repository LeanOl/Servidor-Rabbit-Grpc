using System.Net.Sockets;

namespace Protocol
{
    public class DataHandler
    {
        private readonly Socket _socket;
        public DataHandler(Socket socket)
        {
            _socket= socket;
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