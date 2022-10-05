using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTool : ScriptableObject
{
    public int Uses;
    public bool UsesForce = false;
    public GameObject toolPrefab;
    public Vector3 EquippedOffset = new(0f, 0.85f, 0f);
    public virtual void Use(Transform unit, float force = 1)
    {
        if (Uses == -1) return;
        if (--Uses == 0)
            Destroy(this);
    }
}
