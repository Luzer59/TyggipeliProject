using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float mass;
    public float dragSurfaceSize;
    public float dragCoefficient;
    public float radius;

    private Vector3 velocity;
    private bool active = false;

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

            transform.Translate(velocity * Time.deltaTime);

            float terrainY = MyTerrain.instance.GetMapPosition(MyTerrain.instance.GetNormalizedPos(transform.position.x)).y;

            if (transform.position.y <= terrainY)
            {
                //MyTerrain.instance.ModifyTerrainMesh(MyTerrain.ModifyType.Cut, transform.position, radius);
                Destroy(gameObject);
            }
        }
    }
}
