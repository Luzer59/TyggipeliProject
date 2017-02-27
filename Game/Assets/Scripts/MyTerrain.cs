using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class MyTerrain : MonoBehaviour
{
    public static MyTerrain instance;

    public enum ModifyType { Cut }

    public int detail;
    public float size;
    public int hillCount;
    public int minHillHeight;
    public int maxHillHeight;
    public Material terrainMaterial;
    public int terrainLowerBorder;
    public float terrainMeshXOffset;
    public Vector3 terrainPositionOffset;

    private Mesh _terrain;
    private float[] _map;
    private float _highestPoint = 0f;

    public float[] map
    {
        get { return _map; }
        set { map = value; }
    }

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

    public void GenerateMap()
    {
        GenerateBaseData();
        GenerateHills();
        GenerateTerrainMesh();
        AssingTerrainMesh();
        CenterTerrain();
    }

    public void GenerateMap(float[] map)
    {
        _map = map;
        GenerateTerrainMesh();
        AssingTerrainMesh();
        CenterTerrain();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GenerateMap();
        }
    }

    void GenerateBaseData()
    {
        _map = new float[detail];
    }

    void GenerateHills()
    {
        int[] hillPos = new int[hillCount + 2];
        hillPos[0] = 0;
        _highestPoint = 0f;

        for (int i = 1; i < hillPos.Length - 1; i++)
        {
            hillPos[i] = Random.Range(0, _map.Length);
            int sign = Mathf.RoundToInt(Mathf.Sign(Random.Range(-1f, 1f)));
            _map[hillPos[i]] = Random.Range(sign * minHillHeight, sign * maxHillHeight);
        }

        hillPos[hillPos.Length - 1] = _map.Length - 1;
        System.Array.Sort(hillPos);

        for (int i = 0; i < hillPos.Length - 1; i++)
        {
            int startPos = hillPos[i];
            int endPos = hillPos[i + 1];
            int segments = endPos - startPos;

            if (segments < 2)
            {
                continue;
            }

            float partLenght = 1f / ((float)segments);
            float startHeight = _map[startPos];
            float endHeight = _map[endPos];

            for (int p = 0; p < segments + 1; p++)
            {
                float t = p * partLenght;
                float interpValue = t * t * (3f - 2f * t);
                float interp = Mathf.Lerp(startHeight, endHeight, interpValue);
                _map[startPos + p] = interp;
                if (interp > _highestPoint)
                {
                    _highestPoint = interp;
                }
            }
        }
    }

    void GenerateTerrainMesh()
    {
        _terrain = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = -1; i < _map.Length - 2; i++)
        {
            float x = size * (terrainMeshXOffset + i + 1);
            vertices.Add(new Vector3(x, _map[i + 1], 0f));
            vertices.Add(new Vector3(x, terrainLowerBorder, 0f));

            if (i > 0)
            {
                triangles.Add(i * 2);
                triangles.Add(i * 2 + 2);
                triangles.Add(i * 2 + 1);
                triangles.Add(i * 2 + 2);
                triangles.Add(i * 2 + 3);
                triangles.Add(i * 2 + 1);
            }
        }

        _terrain.SetVertices(vertices);
        _terrain.SetTriangles(triangles, 0);
        _terrain.name = "Terrain";
    }

    void UpdateTerrainVertexHeight(int start, float[] height)
    {
        Vector3[] vertices = _terrain.vertices;
        int counter = 0;
        for (int i = start; i < height.Length; i++, counter++)
        {
            _map[i] = height[counter];
            Vector3 vert = vertices[i * 2];
            vert.y = height[counter];
            vertices[i * 2] = vert;
        }
        _terrain.vertices = vertices;
    }

    // EI TOIMI, KORJAA!!!! (poistettu käytöstä)
    public void ModifyTerrainMesh(ModifyType type, Vector2 worldpoint, float radius)
    {
        switch (type)
        {
            case ModifyType.Cut:
                float mapHitpoint = GetNormalizedPos(worldpoint.x) * (detail - 1);
                int firstPoint = Mathf.CeilToInt(GetNormalizedPos(worldpoint.x - radius) * (detail - 1));
                int lastPoint = Mathf.FloorToInt(GetNormalizedPos(worldpoint.x + radius) * (detail - 1));

                //print(firstPoint + " :: " + lastPoint);

                List<float> newVertexHeight = new List<float>();
                for (int i = firstPoint; i < lastPoint + 1; i++)
                {
                    float newY = worldpoint.y - Mathf.Sqrt(Mathf.Pow(Mathf.Abs(mapHitpoint - i), 2) - 1);
                    newVertexHeight.Add(newY);
                    //print("new: " + newY + "  old: " + map[i]);
                }

                UpdateTerrainVertexHeight(firstPoint, newVertexHeight.ToArray());
                break;

            default:
                break;
        }
    }

    void AssingTerrainMesh()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = _terrain;
        mr.material = terrainMaterial;
    }

    void CenterTerrain()
    {
        Vector3 terrainCenter = _terrain.bounds.center;
        transform.position = terrainPositionOffset - terrainCenter;
    }

    public float GetNormalizedPos(float xPos)
    {
        return Mathf.InverseLerp(-detail * size / 2, detail * size / 2, xPos);
    }

    public Vector2 GetMapPosition(float normalizedPos)
    {
        float mapPos = (map.Length - 1) * normalizedPos;
        float y = Mathf.Lerp(map[Mathf.FloorToInt(mapPos)], map[Mathf.CeilToInt(mapPos)], mapPos - Mathf.Floor(mapPos)) + transform.position.y;
        float x = -detail * size / 2 + detail * size * normalizedPos;
        return new Vector2(x,y);
    }

    public bool GetMapPosition(float normalizedPos, out Vector2 pos, float cliffDetectionRange, float cliffAngleLimit)
    {
        float mapPos = (map.Length - 1) * normalizedPos;

        float y = Mathf.Lerp(map[Mathf.FloorToInt(mapPos)], map[Mathf.CeilToInt(mapPos)], mapPos - Mathf.Floor(mapPos)) + transform.position.y;
        float x = -detail * size / 2 + detail * size * normalizedPos;

        pos = Vector2.zero;

        if (mapPos < cliffDetectionRange || mapPos > map.Length - 1 - cliffDetectionRange)
        {
            return false;
        }

        float yPrev = Mathf.Lerp(map[Mathf.FloorToInt(mapPos - cliffDetectionRange)], map[Mathf.CeilToInt(mapPos - cliffDetectionRange)], mapPos - Mathf.Floor(mapPos)) + transform.position.y;
        float xPrev = -detail * size / 2 + detail * size * normalizedPos - cliffDetectionRange;
        Vector2 dirPrev = new Vector2(xPrev - x, yPrev - y);

        float yNext = Mathf.Lerp(map[Mathf.FloorToInt(mapPos + cliffDetectionRange)], map[Mathf.CeilToInt(mapPos + cliffDetectionRange)], mapPos - Mathf.Floor(mapPos)) + transform.position.y;
        float xNext = -detail * size / 2 + detail * size * normalizedPos + cliffDetectionRange;
        Vector2 dirNext = new Vector2(xNext - x, yNext - y);

        pos = new Vector2(x, y);

        float anglePrev = Vector2.Angle(Vector2.up, dirPrev);
        float angleNext = Vector2.Angle(Vector2.up, dirNext);

        if (anglePrev < 90f + cliffAngleLimit && anglePrev > 90f - cliffAngleLimit && angleNext < 90f + cliffAngleLimit && angleNext > 90f - cliffAngleLimit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
        
    // TODO tee normaalin haku metodi
}
