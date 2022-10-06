using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "Tools/Projectile", order = 1)]
public class ProjectileTool : BaseTool
{
    public GameObject Projectile;
    public float ShootForce;
    public override void Use(Transform unit, float force = 1)
    {
        GameObject go = Instantiate(Projectile, EquippedTransform.position, EquippedTransform.rotation); 
        go.GetComponent<Rigidbody>().velocity = force * ShootForce * (EquippedTransform.forward + Vector3.up).normalized;
        base.Use(unit, force);
    }
}
