using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableBlock : Damageable
{
    public override void LethalDamage()
    {
        //Callback to save the block
        GetComponentInParent<IslandManager>().RemoveCube(gameObject);
        base.LethalDamage();
    }
}
