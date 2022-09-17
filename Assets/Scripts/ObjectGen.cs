using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGen : MonoBehaviour
{
    [SerializeField] float SpawnHeight = 4;
    [SerializeField] private SpawnableObject[] _objects;
    List<Vector2Int> _avaliablePos;

    public IEnumerator GenerateObjects(bool[,] placeableArea)
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

                Vector3 place = new(pos.x, SpawnHeight, pos.y);
                Instantiate(obj.Prefab, place, Quaternion.identity, transform);

                yield return new WaitForEndOfFrame();
            }
        }
    }
    public List<GameObject> More(string name, int amount)
    {
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
            Vector3 place = new(pos.x, SpawnHeight, pos.y);
            output.Add(Instantiate(obj.Prefab, place, Quaternion.identity, transform));
        }
        return output;
    }
}
[System.Serializable]
public struct SpawnableObject
{
    public string Name;
    public GameObject Prefab;
    public Vector3Int Size;
    public int Amount;
}
