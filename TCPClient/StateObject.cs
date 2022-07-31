using System.Net.Sockets;
using System.Text;

namespace TCPClient
{
    public class StateObject
    {
        // Client socket
        public readonly Socket currentSocket = null;
        // Size of received buffer 
        public const int BufferSize = 1024;
        // Received buffer. 
        public byte[] buffer = new byte[BufferSize];
        // Received data string
        public StringBuilder sb = new StringBuilder();

        public StateObject(Socket socket)
        {
            currentSocket = socket;
        }

        public void CleanBuffer()
        {
            sb = new StringBuilder();
        }
    }
}
