using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTool : ScriptableObject
{
    public int Uses;
    public bool UsesForce = false;
    public GameObject ToolPrefab;
    public Sprite ToolIcon;
    internal Transform EquippedTransform;
    public Vector3 EquippedOffset = new(0f, 0.85f, 0f);
    public virtual void Use(Transform unit, float force = 1)
    {
        if (Uses == -1) return;
        Uses--;
    }
}
