using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MapGenerator
{
    public int detail = 300;
    public float size = 0.4f;
    public int hillCount = 7;
    public int minHillHeight = 10;
    public int maxHillHeight = 15;
    private float _highestPoint = 0f;

    public float[] GenerateMap()
    {
        Random rnd = new Random();
        float[] _map = new float[detail];

        int[] hillPos = new int[hillCount + 2];
        hillPos[0] = 0;
        _highestPoint = 0f;

        for (int i = 1; i < hillPos.Length - 1; i++)
        {
            hillPos[i] = rnd.Next(0, _map.Length);
            int sign = Sign(rnd.Next(-100, 100));
            _map[hillPos[i]] = sign * rnd.Next(minHillHeight, maxHillHeight);
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
                float interp = Lerp(startHeight, endHeight, interpValue);
                _map[startPos + p] = interp;
                if (interp > _highestPoint)
                {
                    _highestPoint = interp;
                }
            }
        }

        return _map;
    }

    int RoundToInt(float value)
    {
        if (value - ((int)value) >= 0.5f)
        {
            return (int)(value + 1);
        }
        else
        {
            return (int)value;
        }
    }

    float Lerp(float a, float b, float t)
    {
        return a * t + b * (1 - t);
    }

    int Sign(float value)
    {
        if (value >= 0f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}
