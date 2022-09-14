using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapgen : MonoBehaviour
{
    [SerializeField] GameObject _cubePrefab;
    [Space]
    [SerializeField] bool _testTrigger;
    private void Start()
    {
        GenerateMap();
    }
    private void Update()
    {
        if (_testTrigger)
        {
            _testTrigger = false;
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
            GenerateMap();
        }
    }
    public void GenerateMap()
    {
        bool[,] map = Cellular.GenerateArray();
        StartCoroutine(GenerationRoutine(map));
    }
    IEnumerator GenerationRoutine(bool[,] map)
    {
        int prog = 0;
        int goal = map.Length;
        Vector3Int offset = new(-map.GetLength(0) / 2, 0, -map.GetLength(1) / 2);
        for (int i = 0; i < 3; i++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y])
                        Instantiate(_cubePrefab, new Vector3(x, i, y) + offset, Quaternion.identity, transform);
                    //if (prog % 50 == 0) 
                    //{
                    //    Debug.Log(goal);
                    //    yield return new WaitForEndOfFrame(); 
                    //}
                }
            }
            map = Cellular.Iterate(map);
        }
        yield return new WaitForEndOfFrame();
    }
}
