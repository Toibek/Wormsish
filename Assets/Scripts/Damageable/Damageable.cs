using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] protected int _maxHealth;
    protected int _health;
    private void Start()
    {
        _health = _maxHealth;
    }
    public void Damage(int damage)
    {
        _health -= damage;
        if (_health < 0)
            LethalDamage();
    }
    public void Heal(int health)
    {
        _health += health;
    }
    public virtual void LethalDamage()
    {
        Destroy(gameObject);
    }
}
