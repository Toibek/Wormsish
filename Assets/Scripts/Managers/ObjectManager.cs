using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private SpawnableObject[] _objects;

    List<Vector2Int> _avaliablePos;
    List<GameObject> _activeObjects = new();

    int GetObjectInt(string name)
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            if (name == _objects[i].Name)
                return i;
        }
        return -1;
    }
    public void ClearObjects()
    {
        for (int i = _activeObjects.Count - 1; i >= 0; i--)
            Decomission(_activeObjects[i]);
    }
    public void SpawnObjects(bool[,] placeableArea)
    {
        Vector2Int offset = new(-placeableArea.GetLength(0) / 2, -placeableArea.GetLength(1) / 2);
        _avaliablePos = new List<Vector2Int>();
        for (int x = 0; x < placeableArea.GetLength(0); x++)
        {
            for (int y = 0; y < placeableArea.GetLength(1); y++)
            {
                if (placeableArea[x, y]) _avaliablePos.Add(new(x + offset.x, y + offset.y));
            }
        }
        for (int i = 0; i < _objects.Length; i++)
        {
            SpawnableObject obj = _objects[i];
            for (int amn = 0; amn < obj.Amount; amn++)
            {
                Vector2Int pos = _avaliablePos[Random.Range(0, _avaliablePos.Count)];
                _avaliablePos.Remove(pos);
                Ray ray = new(new(pos.x, 100, pos.y), Vector3.down);
                Vector3 place = new(pos.x, 0, pos.y);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
                    place.y = hit.point.y + 0.5f;
                else
                    place.y = 10;
                GameObject go = obj.GetObject(place, transform);
                go.GetComponent<DamageableObject>().ObjectManager = this;
                go.name = obj.Name;
                _activeObjects.Add(go);
            }
        }
    }
    public List<GameObject> More(string name, int amount)
    {
        Debug.Log("Spawning: " + amount + " " + name);
        SpawnableObject obj = _objects[0];
        for (int i = 0; i < _objects.Length; i++)
        {
            if (name.ToLower() == _objects[i].Name.ToLower())
            {
                obj = _objects[i]; break;
            }
        }
        if (obj.Name.ToLower() != name.ToLower())
        {
            Debug.LogWarning("Object not found: " + name);
            return null;
        }
        List<GameObject> output = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            Vector2Int pos = _avaliablePos[Random.Range(0, _avaliablePos.Count)];
            _avaliablePos.Remove(pos);
            Ray ray = new(new(pos.x, 100, pos.y), Vector3.down);
            Vector3 place;
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
                place = hit.point + new Vector3(0, 0.5f, 0);
            else
                place = new(pos.x, 10, pos.y);
            GameObject go = obj.GetObject(place, transform);
            output.Add(go);
            go.GetComponent<DamageableObject>().ObjectManager = this;
            go.name = name;
        }
        _activeObjects.AddRange(output);
        return output;
    }
    public void Decomission(GameObject go)
    {
        _activeObjects.Remove(go);
        SpawnableObject obj = new();
        for (int i = 0; i < _objects.Length; i++)
        {
            if (go.name == _objects[i].Name)
            {
                obj = _objects[i];
                break;
            }
        }
        obj.DeactivateObject(go);
        if (string.IsNullOrEmpty(obj.Name)) Debug.LogWarning("Unexpected object trying to pool");
    }
}
[System.Serializable]
public class SpawnableObject : object
{
    public string Name;
    public GameObject Prefab;
    public Vector3Int Size;
    public int Amount;
    internal List<GameObject> InactiveObjects;
    public GameObject GetObject(Vector3 position, Transform parent)
    {
        if (InactiveObjects == null) InactiveObjects = new List<GameObject>();
        if (InactiveObjects.Count > 0)
        {
            GameObject go = InactiveObjects[0];
            go.transform.position = position;
            go.transform.SetParent(parent);
            InactiveObjects.Remove(go);
            go.SetActive(true);
            return go;
        }
        else
            return GameObject.Instantiate(Prefab, position, Quaternion.identity, parent);

    }
    public void DeactivateObject(GameObject go)
    {
        if (InactiveObjects == null) InactiveObjects = new List<GameObject>();
        InactiveObjects.Add(go);
        go.SetActive(false);
    }
}
