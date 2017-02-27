using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WindMeter : MonoBehaviour
{
    public RectTransform meter;
    public GameController gc;
    public Text text;

    void Update()
    {
        meter.localRotation = Quaternion.LookRotation(Vector3.forward, gc.wind);
        text.text = gc.wind.magnitude.ToString();
    }
}
