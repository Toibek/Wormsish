using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTools : MonoBehaviour
{
    public Unit Unit;
    public BaseTool ActiveTool
    {
        get { return tools[_activeTool]; }
    }
    public List<BaseTool> tools = new();
    int _activeTool;

    Transform toolTransform;

    Coroutine forceRoutine;

    float _force = 0;
    public void ChangeTool(int toolToUse)
    {
        _activeTool = toolToUse;

    }
    public void NextTool()
    {
        if (tools.Count == 0)
        {
            //notools
            return;
        }
        _activeTool = (_activeTool + 1) % tools.Count;
    }
    public void Rotation(float rot)
    {
        if (toolTransform != null)
            toolTransform.rotation = Quaternion.Euler(0, rot, 0);
    }
    public void ShootStart()
    {
        if (ActiveTool.UsesForce)
        {
            forceRoutine = StartCoroutine(ChargeForce());
        }
    }
    IEnumerator ChargeForce()
    {
        while (true)
        {
            for (_force = 0; _force < 1; _force += Time.deltaTime * 2)
            {
                Unit.PlayerManager.DisplayForce(_force);
                yield return new WaitForEndOfFrame();
            }
            for (_force = 1; _force >= 0; _force -= Time.deltaTime * 2)
            {
                Unit.PlayerManager.DisplayForce(_force);
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public void ShootEnd()
    {
        if (ActiveTool.UsesForce)
        {
            StopCoroutine(forceRoutine);
            ActiveTool.Use(transform);
        }
        else
        {
            ActiveTool.Use(transform);
        }
        if (ActiveTool == null) NextTool();
    }
}
