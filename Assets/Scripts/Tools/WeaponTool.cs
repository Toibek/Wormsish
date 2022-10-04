using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Tools/Weapon", order = 1)]
public class WeaponTool : BaseTool
{
    public override void Use(Transform unit, float force = 1)
    {

        base.Use(unit, force);
    }
}
