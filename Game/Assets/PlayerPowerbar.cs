using UnityEngine;
using System.Collections;

public class PlayerPowerbar : MonoBehaviour
{
    private SpriteRenderer image;
    public Player player;

    private float startScale = 1f;

    void Start()
    {
        image = GetComponent<SpriteRenderer>();
        startScale = transform.localScale.x;
    }

    void Update()
    {
        Vector3 newScale = new Vector3(6 * (player.GetPower() / 100f), 1f, 1f);
        transform.localScale = newScale;
    }
}
