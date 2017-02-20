using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using QuantiCode.Netwokking;

namespace QuantiCode.Netwokking
{
    class Controller
    {
        private Server server;

        public void Start()
        {
            string input;
            while (true)
            {
                server = new Server();
                server.OpenServer(7777);
                Console.WriteLine("Server started. Waiting for connections...");
                RegisterEvents();
                break;
                /*Console.WriteLine("Give port");
                input = Console.ReadLine();
                int port;
                if (int.TryParse(input, out port))
                {
                    server = new Server();
                    server.OpenServer(port);
                    break;
                }*/
            }
            
            while (true)
            {
                server.Update();
                /*for (int i = 0; i < 10; i++)
                    Thread.Sleep(10);
                server.CloseServer();
                break;*/
            }
        }

        void RegisterEvents()
        {
            server.connectionAddedEvent += ConnectionAdded;
            server.connectionClosedEvent += ConnectionClosed;
            server.dataReceivedEvent += DataReceived;
        }

        void ConnectionAdded(uint conId)
        {
            server.SendAll(NetworkMessages.connectionCountChanged, server.connectionCount);
            Console.WriteLine("Connection added with id " + conId);
        }

        void ConnectionClosed(uint conId)
        {
            server.SendAll(NetworkMessages.connectionCountChanged, server.connectionCount);
            Console.WriteLine("Connection closed with id " + conId);
        }

        void DataReceived(string message, object data, uint senderId)
        {
            Console.WriteLine("Data received from " + senderId + " with message: " + message);
        }
    }
}
