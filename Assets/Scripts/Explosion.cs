using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private int _damage = 0;
    private bool _started = false;
    public void StartExplosive(int damage)
    {
        _damage = damage;
        _started = true;
        GetComponent<ParticleSystem>().Play();
    }
    private void OnTriggerStay(Collider other)
    {
        if (_started && other.GetComponent<Damageable>())
        {
            other.GetComponent<Damageable>().Damage(_damage);
        }
    }
}
