using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace QuantiCode.Netwokking
{
    class ClientConnection
    {
        public ClientConnection(Socket socket, Client client)
        {
            _socket = socket;
            _client = client;
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
        private Client _client = null;
        private List<byte[]> _receiveQueue = new List<byte[]>();
        private object _receiveLock = new object();
        private List<byte[]> _sendQueue = new List<byte[]>();
        private object _sendLock = new object();
        private bool _isClosing = false;

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
                    if (_receiveQueue[i].Length > 0)
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
            _isClosing = false;
            _receiveThread = new Thread(ReceiveThreadLoop);
            _sendThread = new Thread(SendThreadLoop);
            _receiveThread.Start();
            _sendThread.Start();
            _client.InvokeConnectedEvent();
        }

        public void Close()
        {
            if (!_isClosing)
            {
                //Console.WriteLine("Connection to {0} was lost", _socket.RemoteEndPoint.ToString());
                _isClosing = true;
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _client.InvokeDisconnectedEvent();
            }
        }

        void ReceiveThreadLoop()
        {
            byte[] bytes;

            while (true)
            {
                try
                {
                    bytes = new byte[_client.bufferSize];

                    _socket.Receive(bytes);
                    //Console.WriteLine("Received data from {0}", _socket.RemoteEndPoint.ToString());

                    if (bytes == null || bytes.Length == 0)
                    {
                        _client.CloseClient();              
                        return;
                    }
                    else
                    {
                        lock (_receiveLock)
                        {
                            _receiveQueue.Add(bytes);
                        }
                    }
                }
                catch (SocketException)
                {
                    _client.CloseClient();
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
                        bytes = new byte[_client.bufferSize];

                        lock (_sendLock)
                        {
                            bytes = _sendQueue[0];
                            _sendQueue.RemoveAt(0);
                        }

                        _socket.Send(bytes);
                    }
                    catch (SocketException)
                    {
                        _client.CloseClient();
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
