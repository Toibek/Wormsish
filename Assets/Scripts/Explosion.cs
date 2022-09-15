using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Damageable>())
        {
            other.GetComponent<Damageable>().Damage(100);
        }
    }
}
