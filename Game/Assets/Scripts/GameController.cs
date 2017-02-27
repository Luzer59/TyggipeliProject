using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public MyTerrain terrainGenerator;
    public GameObject playerPrefab;

    private NetworkClient client;

    public Player p1;
    public Player p2;

    public enum Turn { p1, p2 }
    public Turn turn;

    public Vector2 wind;
    public float maxWind = 1f;

    void Start()
    {
        //client = FindObjectOfType<NetworkClient>();
        //terrainGenerator.GenerateMap(GlobalData.map);
        //client.ListenDataReceivedEvent(GameStart);

        terrainGenerator.GenerateMap();
        GameObject go = Instantiate(playerPrefab);
        p1 = go.GetComponent<Player>();
        p1.p = Turn.p1;
        go = Instantiate(playerPrefab);
        p2 = go.GetComponent<Player>();
        p2.p = Turn.p2;
        p1.Initialize(this);
        p2.Initialize(this);

        p1.TurnStart();
        wind = Random.insideUnitCircle * maxWind;
    }

    public void ChangeTurn()
    {
        if (turn == Turn.p1)
        {
            turn = Turn.p2;
            p2.TurnStart();
        }
        else
        {
            turn = Turn.p1;
            p1.TurnStart();
        }
        wind = Random.insideUnitCircle * maxWind;
    }

    public void GameEnd()
    {
        GlobalController.LoadScene("game");
    }
}
