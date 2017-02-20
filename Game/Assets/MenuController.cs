using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public void OpenMenu(GameObject go)
    {
        go.SetActive(true);
    }

    public void CloseMenu(GameObject go)
    {
        go.SetActive(false);
    }
}
