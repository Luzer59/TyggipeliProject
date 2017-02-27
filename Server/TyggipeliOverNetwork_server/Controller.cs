using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using QuantiCode.Netwokking;

class Controller
{
    private Server server;
    private Logic logic;
    private Thread updateThread;

    public void Start()
    {
        StartServer();
        InputThread();           
    }

    void UpdateThread()
    {
        while (true)
        {
            server.Update();
            logic.Update();
        }
    }

    void StartServer()
    {
        server = new Server();
        server.OpenServer(7777);
        Console.WriteLine("Server started. Waiting for connections...");
        logic = new Logic();
        logic.Initialize(server);
        updateThread = new Thread(UpdateThread);
        updateThread.Start();
    }

    void InputThread()
    {       
        string input;
        while (true)
        {
            // Console.WriteLine("Give port");
            input = Console.ReadLine();
            /*int port;
            if (int.TryParse(input, out port))
            {
                server = new Server();
                server.OpenServer(port);
                break;
            }*/
        }
    }
}
