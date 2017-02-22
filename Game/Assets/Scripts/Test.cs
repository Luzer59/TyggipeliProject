using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
    public float t = 0;

    void Update()
    {
        float asd = t * t * (3f - 2f * t);
        print(asd);
    }
}
