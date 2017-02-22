using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace QuantiCode.Netwokking
{
    class ServerListener
    {
        public ServerListener(Server server)
        {
            _server = server;
        }

        private Thread _listenerThread = null;
        private Socket _listener = null;
        private Server _server = null;
        private int _listenerPort;
        private bool _isClosing = false;

        public void Listen(int port)
        {
            _listenerPort = port;
            _listenerThread = new Thread(ListenThreadLoop);
            _listenerThread.Start();
        }

        void ListenThreadLoop()
        {
            IPEndPoint localEndPoint = new IPEndPoint(NetworkComponent.GetLocalIPAddress(), _listenerPort);
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(10);
                //Console.WriteLine("Ready. Waiting for connections...");
            }
            catch
            {

            }

            while (true)
            {
                try
                {
                    Socket newSocket = _listener.Accept();
                    _server.AddConnection(newSocket);
                }
                catch
                {
                    //Console.WriteLine("Connection timeout/error. Retrying...");
                }
                if (_isClosing)
                {
                    break;
                }
            }
        }

        public void Close()
        {
            _isClosing = true;
            //_listener.Shutdown(SocketShutdown.Both);
            _listener.Close();
        }
    }
}
