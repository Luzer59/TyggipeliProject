using UnityEngine;
using System.Collections;
using QuantiCode.Netwokking;
using System.Net;

public class NetworkServer : MonoBehaviour
{
    private Server _server;

    public static NetworkServer instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartServer(int port)
    {
        _server = new Server();
        _server.OpenServer(port);
    }

    public void CloseServer()
    {
        if (_server != null)
        {
            _server.CloseServer();
            _server = null;
        }
    }

    /*void Update()
    {
        if (_server != null)
        {
            _server.Update();
        }
    }*/
}
