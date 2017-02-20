using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public MyTerrain terrainGenerator;
    public GameObject playerPrefab;
    
    void Start()
    {
        terrainGenerator.GenerateMap();
        Instantiate(playerPrefab);
    }
}
