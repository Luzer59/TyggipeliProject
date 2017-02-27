using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
    private NetworkClient client;
    public bool playerReady = false;

    public void OpenMenu(GameObject go)
    {
        go.SetActive(true);
    }

    public void CloseMenu(GameObject go)
    {
        go.SetActive(false);
    }

    void Start()
    {
        //client = FindObjectOfType<NetworkClient>();
        //client.ListenDataReceivedEvent(GameStart);
    }

    void GameStart(string message, object data)
    {
        if (message == NetworkMessages.mapData)
        {
            GlobalData.map = (float[])data;
            GlobalController.LoadScene("game");
        }
    }

    public void SendPlayerReady()
    {
        if (client.isConnected)
        {
            playerReady = !playerReady;
            client.SendData(NetworkMessages.playerReady, playerReady);
        }
    }

    public void ChangeGaem()
    {
        GlobalController.LoadScene("game");
    }
}
