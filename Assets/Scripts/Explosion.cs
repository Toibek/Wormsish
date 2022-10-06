using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private int _damage = 0;
    private bool _started = false;
    List<Damageable> Damaged = new List<Damageable>();
    public void StartExplosive(int damage)
    {
        _damage = damage;
        _started = true;
        GetComponent<ParticleSystem>().Play();
    }
    private void OnTriggerStay(Collider other)
    {
        Damageable d;
        if (_started && other.TryGetComponent(out d) && !Damaged.Contains(d))
        {
            d.Damage(_damage);
            Damaged.Add(d);
        }
    }
}
