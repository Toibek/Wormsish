using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitDamageable), typeof(UnitMovement))]
public class Unit : MonoBehaviour
{
    public string Name;

    internal PlayerManager PlayerManager;
    internal UnitMovement Movement;
    internal UnitTools Tools;
    internal UnitDamageable Damageable;
    private void Awake()
    {
        Movement = GetComponent<UnitMovement>();
        Movement.Unit = this;
        Damageable = GetComponent<UnitDamageable>();
        Damageable.Unit = this;
        Tools = GetComponent<UnitTools>();
        Tools.Unit = this;
    }
    public void ReportDeath()
    {
        PlayerManager.ReportUnitDeath(this);
    }
}
