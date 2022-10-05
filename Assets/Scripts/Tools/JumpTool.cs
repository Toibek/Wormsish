using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jump", menuName = "Tools/Jump", order = 1)]
public class JumpTool : BaseTool
{
    public override void Use(Transform unit, float force = 1)
    {
        base.Use(unit, force);
    }
}
