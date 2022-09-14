using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapgen : MonoBehaviour
{
    public int Size = 50;
    [Range(0, 100)] public int NoiseDensity = 88;
    [Range(1, 7)] public int Neighbors = 6;
    [Range(0, 20)] public int Iterations = 10;
    [Space]
    [SerializeField] GameObject _cubePrefab;
    private void Start()
    {
        GenerateMap();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
            GenerateMap();
        }
    }
    public void GenerateMap()
    {
        bool[,] map = Cellular.GenerateArray(Size, NoiseDensity, Neighbors, Iterations);
        StartCoroutine(GenerationRoutine(map));
    }
    IEnumerator GenerationRoutine(bool[,] map)
    {
        Vector3Int offset = new(-map.GetLength(0) / 2, 0, -map.GetLength(1) / 2);
        for (int i = 0; i < 3; i++)
        {
            for (int x = i; x < map.GetLength(0) - i; x++)
            {
                for (int y = i; y < map.GetLength(1) - i; y++)
                {
                    if (map[x, y])
                        Instantiate(_cubePrefab, new Vector3(x, i, y) + offset, Quaternion.identity, transform);
                }
            }
            yield return new WaitForEndOfFrame();
            map = Cellular.Iterate(map);
        }
        yield return new WaitForEndOfFrame();
    }
}
