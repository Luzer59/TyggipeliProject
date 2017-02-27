using UnityEngine;
using System.Collections;

public class GlobalData : MonoBehaviour
{
    public static float physicsScale = 0.2f;
    private static float _gravity = 9.81f;
    public static float gravity
    {
        get { return _gravity * physicsScale; }
        set { _gravity = value; }
    }

    public static float[] map;
}
