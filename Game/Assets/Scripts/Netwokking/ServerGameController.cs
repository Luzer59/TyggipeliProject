using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkServer))]
public class ServerGameController : MonoBehaviour
{
    private NetworkServer _server;

    void Start()
    {
        _server = GetComponent<NetworkServer>();
    }
}
