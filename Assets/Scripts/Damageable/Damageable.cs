using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int MaxHealth;
    public int _health;
    protected void Start()
    {
        _health = MaxHealth;
    }
    public void Damage(int damage)
    {
        ChangeHealth(-damage);
    }
    public void Heal(int health)
    {
        ChangeHealth(health);
    }
    public virtual void ChangeHealth(int change)
    {
        _health = Mathf.Clamp(_health + change, 0, MaxHealth);
        if (_health == 0)
            LethalDamage();
    }
    public virtual void LethalDamage()
    {
        Destroy(gameObject);
    }
}
