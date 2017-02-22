using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using QuantiCode.Netwokking;

public class StartClientButton : MonoBehaviour
{
    public InputField portInput;
    public InputField ipInput;
    public NetworkClient client;

    public void Button()
    {
        string portString = portInput.text;
        string ipString = ipInput.text;
        int port;
        IPAddress ip;

        if (portString == string.Empty && ipString == string.Empty)
        {
            port = 7777;
            ip = NetworkComponent.GetLocalIPAddress();
        }
        else
        {
            if (!int.TryParse(portString, out port))
            {
                Debug.LogWarning("portti meni vituix");
                return;
            }

            if (!IPAddress.TryParse(ipString, out ip))
            {
                Debug.LogWarning("ip meni vituix");
                return;
            }
        }

        client.StartClient(ip, port);
    }
}
