using UnityEngine;
using System.Collections;
using QuantiCode.Netwokking;
using System.Net;
using System;

public class NetworkClient : MonoBehaviour
{
    private Client _client;

    public static NetworkClient instance;
    public event Action clientCreatedEvent = delegate { };

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

    void TestListener(string message, object data)
    {
        Debug.Log("Data received with message: " + message);
    }

    public void ListenDataReceivedEvent(Action<string, object> listener)
    {
        if (_client != null)
        {
            _client.dataReceivedEvent += listener;
        }
    }

    public void StartClient(IPAddress ip, int port)
    {
        _client = new Client();
        _client.dataReceivedEvent += TestListener;
        clientCreatedEvent.Invoke();
        _client.OpenClient(ip, port);     
        Debug.Log("Client started");
    }

    public void CloseClient()
    {
        if (_client != null)
        {
            _client.CloseClient();
            _client = null;
            Debug.Log("Client closed");
        }
    }

    public void SendData(string message, object data)
    {
        _client.Send(message, data);
    }

    void Update()
    {
        if (_client != null)
        {
            _client.Update();
        }
    }

    void OnApplicationQuit()
    {
        CloseClient();
    }
}
