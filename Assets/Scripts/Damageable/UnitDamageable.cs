using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDamageable : Damageable
{
    internal Unit Unit;
    internal Material Material;
    public override void ChangeHealth(int change)
    {
        base.ChangeHealth(change);
        //Debug.Log(Material);
        //Debug.Log(Material.HasFloat("_Health"));
        Material.SetFloat("_Health", (float)_health / (float)MaxHealth);
    }
    public override void LethalDamage()
    {
        Unit.ReportDeath();
        base.LethalDamage();
    }
}
