using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDamageable : Damageable
{
    internal Unit Unit;

    public override void LethalDamage()
    {
        Unit.ReportDeath();
        base.LethalDamage();
    }
}
