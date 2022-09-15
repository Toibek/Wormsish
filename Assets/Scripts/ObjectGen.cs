using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGen : MonoBehaviour
{
    [SerializeField] private SpawnableObject[] _objects;

    public IEnumerator GenerateObjects(bool[,] placeableArea)
    {
        Vector2Int offset = new(-placeableArea.GetLength(0) / 2, -placeableArea.GetLength(1) / 2);
        List<Vector2Int> avaliablePos = new List<Vector2Int>();
        for (int x = 0; x < placeableArea.GetLength(0); x++)
        {
            for (int y = 0; y < placeableArea.GetLength(1); y++)
            {
                if (placeableArea[x, y]) avaliablePos.Add(new(x, y));
            }
        }
        for (int i = 0; i < _objects.Length; i++)
        {
            SpawnableObject obj = _objects[i];
            for (int amn = 0; amn < obj.Amount; amn++)
            {
                Vector2Int pos = avaliablePos[Random.Range(0, avaliablePos.Count)];
                avaliablePos.Remove(pos);

                Vector3 place = new(offset.x + pos.x, 4, offset.y + pos.y);
                Instantiate(obj.Prefab, place, Quaternion.identity, transform);

                yield return new WaitForEndOfFrame();
            }
        }
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
