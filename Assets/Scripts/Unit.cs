using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitDamageable),typeof(Movement))]
public class Unit : MonoBehaviour
{
    public string Name;

    internal PlayerManager PlayerManager;
    internal Movement Movement;
    internal UnitDamageable Damageable;
    private void Awake()
    {
        Movement = GetComponent<Movement>();
        Movement.Unit = this;
        Damageable = GetComponent<UnitDamageable>();
        Damageable.Unit = this;
    }
    public void ReportDeath()
    {
        PlayerManager.ReportUnitDeath(this);
    }
}
