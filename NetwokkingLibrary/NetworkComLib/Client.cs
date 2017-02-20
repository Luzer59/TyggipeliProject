using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QuantiCode.Netwokking
{
    public class Client : NetworkComponent
    {
        private bool _isClientRunning = false;
        public bool isClientRunning
        {
            get
            {
                return _isClientRunning;
            }
        }

        public event Action<string, object> dataReceivedEvent = delegate { };
        public event Action connectedEvent = delegate { };
        public event Action disconnectedEvent = delegate { };

        internal ClientConnection connection = null;
        internal ClientSender sender = null;

        public void OpenClient(IPAddress ipAddress, int port)
        {
            if (!_isClientRunning)
            {
                _isClientRunning = true;
                //Console.WriteLine("Client starting");
                _port = port;
                sender = new ClientSender(this);
                sender.Connect(ipAddress, port);
            }
        }

        public void CloseClient()
        {
            if (_isClientRunning)
            {
                _isClientRunning = false;
                //Console.WriteLine("Client closing");
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        public void Send(string message, object data)
        {
            connection.Send(message, data);
        }

        public void Update()
        {
            if (connection != null)
            {
                string[] message;
                object[] data;
                connection.Receive(out message, out data);
                for (int i = 0; i < message.Length; i++)
                {
                    dataReceivedEvent.Invoke(message[i], data[i]);
                }
            }
        }

        internal void InvokeConnectedEvent()
        {
            connectedEvent.Invoke();
        }

        internal void InvokeDisconnectedEvent()
        {
            disconnectedEvent.Invoke();
        }
    }
}
