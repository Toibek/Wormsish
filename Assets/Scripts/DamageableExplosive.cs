using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableExplosive : Damageable
{
    [SerializeField] private GameObject _prefabExplosion;
    public override void LethalDamage()
    {
        Instantiate(_prefabExplosion, transform.position, Quaternion.identity);
        base.LethalDamage();
    }
}
