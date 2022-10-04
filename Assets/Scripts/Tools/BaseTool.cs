using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTool : ScriptableObject
{
    public int Uses;
    public bool UsesForce = false;
    public GameObject toolEquippedPrefab;
    public virtual void Use(Transform unit, float force = 1)
    {
        if (Uses == -1) return;
        if (--Uses == 0)
            Destroy(this);
    }
}
