using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableExplosive : DamageableObject
{
    [SerializeField] private GameObject _prefabExplosion;
    [SerializeField] private int _damage;
    public override void LethalDamage()
    {
        GameObject go = Instantiate(_prefabExplosion, transform.position, Quaternion.identity);
        go.GetComponent<Explosion>().StartExplosive(_damage);
        base.LethalDamage();
    }
}
