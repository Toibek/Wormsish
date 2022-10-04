using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitDamageable), typeof(UnitMovement))]
public class Unit : MonoBehaviour
{
    public string Name;
    public bool Active;
    internal Transform Camera;
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
    public void Rotate()
    {
        Vector3 org = transform.position;
        Vector3 cam = Camera.position;
        Vector3 dif = (new Vector3(org.x, 0, org.z) - new Vector3(cam.x, 0, cam.z)).normalized;
        float rot = Mathf.Atan2(dif.x, dif.z) * Mathf.Rad2Deg;
        rot = Mathf.Round(rot / 45) * 45;
        //Movement.Rotation(rot);
        Tools.Rotation(rot);
    }
}
