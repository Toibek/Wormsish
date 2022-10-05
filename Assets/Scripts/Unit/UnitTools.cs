using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTools : MonoBehaviour
{
    public Unit Unit;
    public BaseTool previousTool
    {
        get
        {
            int t = _activeTool;
            if (tools.Count < 3) return null;
            if (--t < 0) t = tools.Count - 1;
            return tools[t];

        }
    }
    public BaseTool ActiveTool
    {
        get
        {
            if (tools.Count == 0) return null;
            return tools[_activeTool];
        }
    }
    public List<BaseTool> tools = new();

    public BaseTool NextTool
    {
        get
        {
            int t = _activeTool;
            if (tools.Count < 2) return null;
            if (++t <= tools.Count) t = 0;
            return tools[t];
        }
    }

    GameObject ToolObject
    {
        get
        {
            if (toolObjects.Count > 0)
                return toolObjects[_activeTool];
            return null;
        }
    }
    List<GameObject> toolObjects = new();
    int _activeTool;

    Coroutine forceRoutine;

    float _force = 0;
    public void AddTool(BaseTool tool)
    {
        for (int i = 0; i < tools.Count; i++)
        {
            if (tools[i].name == tool.name)
            {
                tools[i].Uses += tool.Uses;
                return;
            }
        }
        tools.Add(tool);
        toolObjects.Add(Instantiate
            (
            tool.toolPrefab,
            transform.position + tool.EquippedOffset,
            Quaternion.identity,
            transform
            ));
    }
    public void ChangeTool(int toolToUse)
    {
        _activeTool = toolToUse;
    }
    public void SwitchTool(float val)
    {
        int dir = (int)Mathf.Sign(val);
        if (tools.Count == 0)
        {
            return;
        }
        _activeTool = (_activeTool + 1) % tools.Count;
    }
    public void Rotation(float rot)
    {
        if (ToolObject != null)
            ToolObject.transform.rotation = Quaternion.Euler(0, rot, 0);
    }
    public bool ShootStart()
    {
        if (ActiveTool == null) return false;
        if (ActiveTool.UsesForce)
        {
            forceRoutine = StartCoroutine(ChargeForce());
            return true;
        }
        return false;
    }
    IEnumerator ChargeForce()
    {
        _force = 0;
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
        if (ActiveTool == null) return;
        BaseTool tool = ActiveTool;
        if (tool.UsesForce)
        {
            StopCoroutine(forceRoutine);
            tool.Use(transform, _force);
        }
        else
        {
            tool.Use(transform);
        }
        if (tool == null)
        {
            tools.RemoveAt(_activeTool);
            Destroy(toolObjects[_activeTool]);
            toolObjects.RemoveAt(_activeTool);
        }
    }
}
