using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPClient
{
    public class Client
    {
        private readonly IPAddress ipAddress;
        private readonly int port;

        private Socket socket;

        public delegate void DataReceivedEventHandler(string data);
        public event DataReceivedEventHandler OnDataRecievedEvent;

        public delegate void OnConnectEventHandler(bool status);
        public event OnConnectEventHandler OnConnectEvent;

        public Client(string _ip, int _port)
        {
            ipAddress = IPAddress.Parse(_ip);
            port = _port;
        }

        public void Connect()
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    Thread.Sleep(10);
                    socket.Close();
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint epServer = new IPEndPoint(ipAddress, port);

                // Connect to server non-Blocking method
                socket.Blocking = false;
                AsyncCallback onconnect = new AsyncCallback(OnConnect);
                socket.BeginConnect(epServer, onconnect, socket);
            }
            catch (Exception ex)
            {
                OnConnectEvent(false);
                Logger.Log(LogLevel.Exception, $"Socket connection failure: {ex.Message}");
            }
        }

        public bool IsConnected()
        {
            if (socket != null)
            {
                return socket.Connected;
            }
            return false;
        }

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                Socket _socket = (Socket)ar.AsyncState;
                if (_socket.Connected)
                {
                    SetupRecieveCallback(_socket);
                    OnConnectEvent(true);
                }
                else
                {
                    OnConnectEvent(false);
                    Logger.Log(LogLevel.Error, "Cannot establish the socket connection");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Exception, $"Cannot establish the socket connection: {ex.Message}");
            }
        }

        private void SetupRecieveCallback(Socket _socket)
        {
            try
            {
                StateObject state = new StateObject(_socket);
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                state.currentSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, recieveData, state);
            }
            catch (Exception ex)
            {
                Dispose();
                Logger.Log(LogLevel.Exception, $"Recieve callback setup failure: {ex.Message}");
            }
        }

        private void OnRecievedData(IAsyncResult ar)
        {
            if (IsConnected())
            {
                try
                {
                    StateObject state = (StateObject)ar.AsyncState;
                    Socket _socket = state.currentSocket;
                    AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                    _socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, recieveData, state);

                    int bytesRead = _socket.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                        // Notify listeners
                        OnDataRecievedEvent(state.sb.ToString());
                        SetupRecieveCallback(_socket);

                        state.CleanBuffer();
                    }
                    else
                    {
                        Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Dispose();
                    Logger.Log(LogLevel.Exception, $"Recieve operation failure: {ex.Message}");
                }
            }
        }

        public bool Send(string data)
        {
            if (IsConnected())
            {
                try
                {
                    byte[] byteDateLine = Encoding.UTF8.GetBytes(data.ToCharArray());
                    socket.Send(byteDateLine, byteDateLine.Length, 0);
                    return true;
                }
                catch (Exception ex)
                {
                    Dispose();
                    Logger.Log(LogLevel.Exception, $"Data writing operation failure: {ex.Message}");
                }
            }
            return false;
        }

        public bool Send(byte[] data)
        {
            if (IsConnected())
            {
                try
                {
                    socket.Send(data, data.Length, 0);
                    return true;
                }
                catch (Exception ex)
                {
                    Dispose();
                    Logger.Log(LogLevel.Exception, $"Data writing operation failure: {ex.Message}");
                }
            }
            return false;
        }

        public void Dispose()
        {
            if (socket != null && socket.Connected)
            {
                OnConnectEvent(false);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        public void Disconnect()
        {
            if (socket != null && socket.Connected)
            {
                OnConnectEvent(false);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}
