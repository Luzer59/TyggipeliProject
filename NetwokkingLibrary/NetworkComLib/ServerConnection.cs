using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;


namespace QuantiCode.Netwokking
{
    class ServerConnection
    {
        public ServerConnection(Socket socket, Server server)
        {
            _socket = socket;
            _server = server;
        }

        private Socket _socket = null;
        public Socket socket
        {
            get
            {
                return _socket;
            }
        }

        private Thread _receiveThread = null;
        private Thread _sendThread = null;
        private Server _server = null;
        private List<byte[]> _receiveQueue = new List<byte[]>();
        private object _receiveLock = new object();
        private List<byte[]> _sendQueue = new List<byte[]>();
        private object _sendLock = new object();
        private bool _isClosing = false;
        private Random rnd = new Random();
        private uint _id = 0;
        public uint id
        {
            get { return _id; }
        }

        public void Send(string message, object data)
        {
            lock (_sendLock)
            {
                NetworkDataPacket packet = new NetworkDataPacket(message, data);
                _sendQueue.Add(Formating.ObjectToByteArray(packet));
            }
        }

        public void Send(byte[] bytes)
        {
            lock (_sendLock)
            {
                _sendQueue.Add(bytes);
            }
        }

        public void Receive(out string[] message, out object[] data)
        {
            lock (_receiveLock)
            {
                int lenght = _receiveQueue.Count;
                message = new string[lenght];
                data = new object[lenght];

                for (int i = 0; i < lenght; i++)
                {
                    if (_receiveQueue[0] != null && _receiveQueue[0].Length > 0)
                    {
                        NetworkDataPacket packet = (NetworkDataPacket)Formating.ByteArrayToObject(_receiveQueue[0]);
                        message[i] = packet.message;
                        data[i] = packet.data;
                        _receiveQueue.RemoveAt(0);
                    }
                    else
                    {
                        _receiveQueue.RemoveAt(0);
                    }
                }
            }
        }

        public byte[][] Receive()
        {
            byte[][] bytes;

            lock (_receiveLock)
            {
                int lenght = _receiveQueue.Count;
                bytes = new byte[lenght][];

                for (int i = 0; i < lenght; i++)
                {
                    if (_receiveQueue[i].Length > 0)
                    {
                        bytes[i] = _receiveQueue[0];
                        _receiveQueue.RemoveAt(0);
                    }
                    else
                    {
                        _receiveQueue.RemoveAt(0);
                    }
                }
            }

            return bytes;
        }

        public void Open()
        {
            _id = (uint)(rnd.Next(int.MaxValue) + int.MaxValue / 2);
            _isClosing = false;
            _receiveThread = new Thread(ReceiveThreadLoop);
            _sendThread = new Thread(SendThreadLoop);
            _receiveThread.Start();
            _sendThread.Start();
        }

        public void Close()
        {
            if (!_isClosing)
            {
                _isClosing = true;
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
        }

        /*void SendToOthers(byte[] bytes)
        {
            for (int i = 0; i < _server.connections.Count; i++)
            {
                if (_server.connections[i]._socket != _socket)
                {
                    _server.connections[i].Send(bytes);
                }
            }
        }*/

        void ReceiveThreadLoop()
        {
            byte[] bytes;

            while (true)
            {
                try
                {
                    bytes = new byte[_server.bufferSize];

                    _socket.Receive(bytes);
                    //Console.WriteLine("Received data from {0}", _socket.RemoteEndPoint.ToString());
                    //Console.WriteLine(bytes.Length);

                    if (bytes == null || bytes.Length == 0)
                    {
                        _server.CloseConnection(_socket);
                        return;
                    }
                    else
                    {
                        lock (_receiveLock)
                        {
                            _receiveQueue.Add(bytes);
                        }

                        //SendToOthers(bytes);
                    }
                }
                catch (SocketException)
                {
                    _server.CloseConnection(_socket);
                    return;
                }

                if (_isClosing)
                {
                    return;
                }
            }
        }

        void SendThreadLoop()
        {
            byte[] bytes;

            while (true)
            {
                if (_sendQueue.Count > 0)
                {
                    try
                    {
                        bytes = new byte[_server.bufferSize];

                        lock (_sendLock)
                        {
                            bytes = _sendQueue[0];
                            _sendQueue.RemoveAt(0);
                        }

                        _socket.Send(bytes);

                        //Console.WriteLine("Sent data to {0}", _socket.RemoteEndPoint.ToString());
                    }
                    catch (SocketException)
                    {
                        _server.CloseConnection(_socket);
                    }
                }

                if (_isClosing)
                {
                    break;
                }
            }
        }
    }
}
