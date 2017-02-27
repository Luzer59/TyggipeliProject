using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GlobalController : MonoBehaviour
{
    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
