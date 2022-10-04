using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : Damageable
{
    internal SpawnableObject ReturnTo;
    public override void LethalDamage()
    {
        ReturnTo.DeactivateObject(gameObject);
    }
}
