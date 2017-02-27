using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuantiCode.Netwokking;

class Logic
{
    private Server server;

    private List<Player> players = new List<Player>();
    private const int minimunPlayers = 2;
    private enum GameState { Lobby, Running }
    private GameState gameState = GameState.Lobby;
    private float[] map;

    public void Initialize(Server server)
    {
        this.server = server;
        RegisterEvents();
    }

    public void Update()
    {

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
        players.Add(new Player(conId));
        Console.WriteLine("Connection added with id " + conId);
    }

    void ConnectionClosed(uint conId)
    {
        server.SendAll(NetworkMessages.connectionCountChanged, server.connectionCount);
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].id == conId)
            {
                players.RemoveAt(i);
                break;
            }
        }
        Console.WriteLine("Connection closed with id " + conId);
    }

    void DataReceived(string message, object data, uint senderId)
    {
        Console.WriteLine("Data received from " + senderId + " with message: " + message);
        Player sender = GetById(senderId);
        switch (message)
        {
            case NetworkMessages.playerReady:
                PlayerReadyChanged(sender, data);
                break;

            default:
                break;
        }
    }

    Player GetById(uint id)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].id == id)
            {
                return players[i];
            }
        }
        return null;
    }

    void PlayerReadyChanged(Player player, object data)
    {
        if (players.Count >= minimunPlayers)
        {
            player.ready = (bool)data;
        }

        Console.WriteLine(player.id);
        Console.WriteLine(player.ready);

        int playersReady = 0;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ready)
                playersReady++;
        }
        Console.WriteLine(playersReady);
        if (playersReady == players.Count)
            StartGame();
    }

    void StartGame()
    {
        gameState = GameState.Running;
        MapGenerator mg = new MapGenerator();
        map = mg.GenerateMap();
        server.SendAll(NetworkMessages.mapData, map);
    }
}
