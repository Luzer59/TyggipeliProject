using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateConnectionCount : MonoBehaviour
{
    public Text text;

    void Start()
    {
        NetworkClient.instance.clientCreatedEvent += Activate;
    }

    void Activate()
    {
        NetworkClient.instance.ListenDataReceivedEvent(UpdateText);
    }

    void UpdateText(string message, object data)
    {
        if (message == NetworkMessages.connectionCountChanged)
        {
            text.text = ((int)data).ToString();
        }
    }
}
