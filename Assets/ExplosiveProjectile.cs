using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] int _damage;
    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        go.GetComponent<Explosion>().StartExplosive(_damage);
        Destroy(gameObject);
    }
}
