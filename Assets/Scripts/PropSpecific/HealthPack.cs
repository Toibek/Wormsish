using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] float _rps;
    [SerializeField] int _value;
    private void Update()
    {
        transform.Rotate(0, 360*_rps * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Damageable>())
        {
            other.GetComponent<Damageable>().Heal(_value);
            Destroy(gameObject);
        }
    }
}
