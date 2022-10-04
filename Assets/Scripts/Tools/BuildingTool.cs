using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bridge", menuName = "Tools/Bridge", order = 1)]
public class BuildingTool : BaseTool
{
    [Space]
    public int MaxDistance;
    public GameObject Block;

    public override void Use(Transform unit, float force = 1)
    {
        base.Use(unit, force);
    }
}
