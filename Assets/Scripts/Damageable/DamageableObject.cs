using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : Damageable
{
    internal ObjectManager ObjectManager;
    public override void LethalDamage()
    {
        //ObjectManager.
        base.LethalDamage();
    }
}
