using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableBlock : Damageable
{
    public override void LethalDamage()
    {
        //Callback to save the block
        GetComponentInParent<IslandGen>().RemoveCube(gameObject);
        base.LethalDamage();
    }
}
