using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace QuantiCode.Netwokking
{
    class ClientSender
    {
        public ClientSender(Client client)
        {
            _client = client;
        }

        private Client _client = null;
        private bool _isClosing = false;
        private IPAddress _connectionIP;
        private int _connectionPort;
        private Thread _connectionThread;

        public void Connect(IPAddress ipAddress, int port)
        {
            _connectionIP = ipAddress;
            _connectionPort = port;
            _connectionThread = new Thread(ConnectThread);
            _connectionThread.Start();
        }
        public void ConnectThread()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(_connectionIP, _connectionPort);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Console.WriteLine("Ready. Waiting to connect...");

            while (true)
            {
                try
                {
                    socket.Connect(remoteEndPoint);
                    _client.connection = new ClientConnection(socket, _client);
                    _client.connection.Open();
                    //Console.WriteLine("Connected to {0}", remoteEndPoint.ToString());
                    _client.sender = null;
                    return;
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.ToString());
                }
                if (_isClosing)
                {
                    return;
                }
            }
        }
    }
}

