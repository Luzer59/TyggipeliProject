using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_PlayerReadyButton : MonoBehaviour
{
    public Color active;
    public Color inActive;
    public MenuController mc;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    public void ChangeImage()
    {
        if (mc.playerReady)
        {
            image.color = active;
        }
        else
        {
            image.color = inActive;
        }
    }
}
