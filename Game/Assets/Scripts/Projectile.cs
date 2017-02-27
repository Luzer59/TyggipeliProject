using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float mass;
    public float dragSurfaceSize;
    public float dragCoefficient;
    public float radius;
    public float damage;

    private Vector3 velocity;
    private bool active = false;

    private GameController gc;

    public void Initialize(GameController gc)
    {
        this.gc = gc;
    }

    public void Shoot(Vector3 powerDirection)
    {
        velocity = powerDirection * GlobalData.physicsScale * 4;
        active = true;
    }

    void Update()
    {
        if (active)
        {
            velocity -= new Vector3(0f, GlobalData.gravity, 0f);
            float drag = 1f * dragCoefficient * dragSurfaceSize / 2 * Mathf.Pow(velocity.magnitude, 2);
            velocity -= velocity.normalized * drag;
            velocity += (Vector3)gc.wind;

            transform.Translate(velocity * Time.deltaTime);

            float terrainY = MyTerrain.instance.GetMapPosition(MyTerrain.instance.GetNormalizedPos(transform.position.x)).y;

            if (transform.position.y <= terrainY)
            {
                //MyTerrain.instance.ModifyTerrainMesh(MyTerrain.ModifyType.Cut, transform.position, radius);
                if (Vector3.Distance(gc.p1.transform.position, transform.position) < radius)
                {
                    gc.p1.TakeDamage(damage);
                }
                if (Vector3.Distance(gc.p2.transform.position, transform.position) < radius)
                {
                    gc.p2.TakeDamage(damage);
                }
                gc.ChangeTurn();
                Destroy(gameObject);
            }
        }
    }
}
