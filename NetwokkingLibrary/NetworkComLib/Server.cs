using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace QuantiCode.Netwokking
{
    public class Server : NetworkComponent
    {
        private bool _isServerRunning = false;
        public bool isServerRunning
        {
            get
            {
                return _isServerRunning;
            }
        }

        public event Action<string, object, uint> dataReceivedEvent = delegate { };
        public event Action<uint> connectionAddedEvent = delegate { };
        public event Action<uint> connectionClosedEvent = delegate { };

        internal List<ServerConnection> connections = new List<ServerConnection>();
        public int connectionCount
        {
            get { return connections.Count; }
        }

        private ServerListener _listener = null;

        public void OpenServer(int port)
        {
            if (!_isServerRunning)
            {
                //Console.WriteLine("Server starting");
                _port = port;
                _listener = new ServerListener(this);
                _listener.Listen(_port);
                _isServerRunning = true;
            }  
        }

        public void CloseServer()
        {
            if (isServerRunning)
            {
                //Console.WriteLine("Server closing");
                if (_listener != null)
                {
                    _listener.Close();
                    _listener = null;
                }

                for (int i = 0; i < connections.Count; i++)
                {
                    connections[i].Close();
                }

                connections.Clear();
                _isServerRunning = false;
            }
        }

        internal void AddConnection(Socket newSocket)
        {
            ServerConnection newConnection = new ServerConnection(newSocket, this);
            newConnection.Open();
            connections.Add(newConnection);
            //Console.WriteLine("Connection from {0}", newSocket.RemoteEndPoint.ToString());
            connectionAddedEvent.Invoke(newConnection.id);
        }

        internal void CloseConnection(Socket connectionSocket)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].socket == connectionSocket)
                {
                    connections[i].Close();
                    connectionClosedEvent.Invoke(connections[i].id);
                    connections.RemoveAt(i);
                    return;
                }
            }
        }

        public void SendAll(string message, object data)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Send(message, data);
            }
        }

        public void Send(string message, object data, uint id)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].id == id)
                {
                    connections[i].Send(message, data);
                }  
            }
        }

        public void Update()
        {
            for (int p = 0; p < connections.Count; p++)
            {
                string[] message;
                object[] data;
                connections[p].Receive(out message, out data);
                for (int i = 0; i < message.Length; i++)
                {
                    dataReceivedEvent.Invoke(message[i], data[i], connections[p].id);
                }
            }
        }
    }
}
