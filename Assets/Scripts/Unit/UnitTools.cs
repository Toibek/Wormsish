using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTools : MonoBehaviour
{
    [SerializeField] float _forceSpeed;
    internal Unit Unit;
    public BaseTool PreviousTool
    {
        get
        {
            int t = _activeTool;
            if (_tools.Count < 3) return null;
            if (--t < 0) t = _tools.Count - 1;
            return _tools[t];

        }
    }
    public BaseTool ActiveTool
    {
        get
        {
            if (_tools.Count == 0) return null;
            return _tools[_activeTool];
        }
    }
    public BaseTool NextTool
    {
        get
        {
            int t = _activeTool;
            if (_tools.Count < 2) return null;
            if (++t <= _tools.Count) t = 0;
            return _tools[t];
        }
    }
    private List<BaseTool> _tools = new();


    private GameObject ToolObject
    {
        get
        {
            if (toolObjects.Count > 0)
                return toolObjects[_activeTool];
            return null;
        }
    }
    private List<GameObject> toolObjects = new();
    private int _activeTool
    {
        get { return _at; }
        set
        {
            _at = value;
            for (int i = 0; i < _tools.Count; i++)
            {
                if (i != _at)
                    _tools[i].EquippedTransform.gameObject.SetActive(false);
                else
                    _tools[i].EquippedTransform.gameObject.SetActive(true);
            }
        }
    }
    private int _at;

    private Coroutine forceRoutine;

    private float _force = 0;

    public void AddTool(BaseTool tool)
    {
        for (int i = 0; i < _tools.Count; i++)
        {
            if (_tools[i].name == tool.name)
            {
                _tools[i].Uses += tool.Uses;
                return;
            }
        }
        _tools.Add(tool);
        Transform t;
        if (tool.EquippedTransform == null)
        {
            t = Instantiate(
                tool.ToolPrefab,
                transform.position + tool.EquippedOffset,
                Quaternion.identity,
                transform).transform;
            if (_tools.Count != 1)
                t.gameObject.SetActive(false);
            tool.EquippedTransform = t;
        }
        else
        {
            t = tool.EquippedTransform;
            t.SetPositionAndRotation(transform.position + tool.EquippedOffset, Quaternion.identity);
            t.parent = transform;
            if (_tools.Count != 1)
                t.gameObject.SetActive(false);
        }
        toolObjects.Add(t.gameObject);
    }
    public void ChangeTool(int toolToUse)
    {
        _activeTool = toolToUse;
    }
    public void ChangeTool(float val)
    {
        int dir = (int)Mathf.Sign(val);
        if (_tools.Count == 0)
        {
            return;
        }
        _activeTool = (_activeTool + 1) % _tools.Count;
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
            for (_force = 0.1f; _force < 1f; _force += Time.deltaTime * _forceSpeed)
            {
                Unit.PlayerManager.DisplayForce(_force);
                yield return new WaitForEndOfFrame();
            }
            for (_force = 1f; _force >= 0.1f; _force -= Time.deltaTime * _forceSpeed)
            {
                Unit.PlayerManager.DisplayForce(_force);
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public bool ShootEnd()
    {
        if (ActiveTool == null) return false;
        BaseTool tool = ActiveTool;
        if (tool.UsesForce)
        {
            StopCoroutine(forceRoutine);
            tool.Use(ActiveTool.EquippedTransform, _force);
            if (tool.Uses == 0) ClearTool(_activeTool);
            return true;
        }
        else
        {
            tool.Use(ActiveTool.EquippedTransform);
            if (tool.Uses == 0) ClearTool(_activeTool);
            return true;
        }
    }
    void ClearTool(int tool)
    {
        _tools.RemoveAt(tool);
        Destroy(toolObjects[tool]);
        toolObjects.RemoveAt(tool);
        _activeTool = 0;
    }
}
