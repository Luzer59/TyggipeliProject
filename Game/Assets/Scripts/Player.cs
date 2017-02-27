using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float maxHealth;
    public float moveSpeed;
    public float aimSpeed;
    public Transform barrel;
    public Transform barrelEnd;
    public float cliffAngleLimit;
    public GameObject projectile;
    public float maxFuel;
    public float powerSpeed;
    public float powerMultiplier = 1f;

    private float health;
    private float position;
    private float angle;
    private float fuel;
    private float power = 50f;
    private bool active = true;

    private GameController gc;
    public GameController.Turn p;

    public float GetHealth()
    {
        return health;
    }

    public float GetPower()
    {
        return power;
    }

    public void Initialize(GameController gc)
    {
        this.gc = gc;
        health = maxHealth;
        for (int i = 0; i < 100; i++)
        {
            float newPos = Mathf.Clamp(Random.value, 0.1f, 0.9f);
            Vector2 pos;
            if (MyTerrain.instance.GetMapPosition(newPos, out pos, 1f, cliffAngleLimit))
            {
                transform.position = pos;
                position = newPos;
                break;
            }
        }
    }

    void Update()
    {
        if (active && gc.turn == p)
        {
            UpdatePosition();
            Aim();
            Shoot();
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            gc.GameEnd();
            Destroy(gameObject);
        }
    }

    public void TurnStart()
    {
        fuel = maxFuel;

        active = true;
    }

    public void TurnEnd()
    {
        active = false;
    }

    void UpdatePosition()
    {

        float newPos = position;

        if (fuel > 0f)
        {
            if (Input.GetKey(KeyCode.D))
            {
                newPos += moveSpeed * Time.deltaTime;
                fuel -= Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                newPos -= moveSpeed * Time.deltaTime;
                fuel -= Time.deltaTime;
            }
        }

        newPos = Mathf.Clamp01(newPos);

        Vector2 pos;
        if (MyTerrain.instance.GetMapPosition(newPos, out pos, 1f, cliffAngleLimit))
        {
            float rightLimit = Camera.main.ViewportToWorldPoint(Vector3.right).x;
            float leftLimit = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
            if (pos.x <= rightLimit && pos.x >= leftLimit)
            {
                transform.position = pos;
                position = newPos;
            }
        }
    }

    void Aim()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            angle += aimSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            angle -= aimSpeed * Time.deltaTime;
        }
        angle = Mathf.Clamp(angle, -90f, 90f);
        barrel.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        if (Input.GetKey(KeyCode.W))
        {
            power += powerSpeed * Time.deltaTime;
            power = Mathf.Clamp(power, 0f, 100f) * powerMultiplier;
        }
        if (Input.GetKey(KeyCode.S))
        {
            power -= powerSpeed * Time.deltaTime;
            power = Mathf.Clamp(power, 0f, 100f) * powerMultiplier;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject go = Instantiate(projectile, barrelEnd.position, Quaternion.identity) as GameObject;
            Projectile proj = go.GetComponent<Projectile>();
            proj.Initialize(gc);
            proj.Shoot(Quaternion.Euler(0f, 0f, angle) * Vector3.up * power);
            TurnEnd();
        }
    }
}
