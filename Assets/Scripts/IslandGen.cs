using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGen : MonoBehaviour
{
    /*
     For future reference-
    If there's anything missing from this it's to optimize the cubes themselves in order to make it take up less
    rendering time. seems to work perfectly fine with the current setup so currently set on the backburner.
     */

    internal bool[,] PlaceableArea;
    [Header("Generation settings")]
    [SerializeField] private int _cubesToPreload;
    [SerializeField] private int Size = 50;
    [SerializeField] private int Layers = 3;
    [SerializeField] [Range(0, 100)] private int NoiseDensity = 88;
    [SerializeField] [Range(1, 7)] private int Neighbors = 6;
    [SerializeField] [Range(0, 20)] private int Iterations = 10;
    [Header("Cube settings")]
    [SerializeField] GameObject _cubePrefab;
    [SerializeField] Material _topMaterial;
    [SerializeField] Material _botMaterial;

    List<GameObject> _inactiveCubes = new List<GameObject>();
    List<GameObject> _activeCubes = new List<GameObject>();

    private Coroutine preLoadRoutine;
    private void Start()
    {
        preLoadRoutine = StartCoroutine(PreloadCubes());
    }
    /// <summary>
    /// Preloading cubes and saving them as inactive so that it can be fetched without needing to instantiate.
    /// </summary>
    private IEnumerator PreloadCubes()
    {
        for (int i = 0; i < _cubesToPreload; i++)
        {
            GameObject go = Instantiate(_cubePrefab, transform);
            _inactiveCubes.Add(go);
            go.SetActive(false);
            if (i % 20 == 0)
                yield return new WaitForEndOfFrame();
        }
    }
    /// <summary>
    /// A coroutine to create the base level, allowing for breaks in the creation.
    /// </summary>
    public IEnumerator GenerationRoutine()
    {
        //Stop preloading if it's active
        if (preLoadRoutine != null)
        {
            StopCoroutine(preLoadRoutine);
            preLoadRoutine = null;
        }
        //Remove active cubes
        for (int i = _activeCubes.Count - 1; i >= 0; i--)
            RemoveCube(_activeCubes[i]);
        yield return new WaitForEndOfFrame();
        //Get the base maps from the Cellular automata generator
        List<bool[,]> maps = new List<bool[,]>();
        maps.Add(Cellular.GenerateArray(-1,Size, NoiseDensity, Neighbors, Iterations));
        for (int i = 0; i < Layers; i++)
        {
            maps.Add(Cellular.Iterate(maps[maps.Count - 1]));
            yield return new WaitForEndOfFrame();
        }
        //Place the cubes in the enabled positions
        Vector3Int offset = new(-maps[0].GetLength(0) / 2, 0, -maps[0].GetLength(1) / 2);
        for (int i = 0; i < maps.Count; i++)
        {
            for (int x = i; x < maps[i].GetLength(0) - i; x++)
            {
                for (int y = i; y < maps[i].GetLength(1) - i; y++)
                {
                    if (maps[i][x, y])
                    {
                        GameObject go = GetCube(new Vector3(x, i, y) + offset);
                        if (i < maps.Count - 1 && maps[i + 1][x, y])
                            go.GetComponent<MeshRenderer>().material = _botMaterial;
                        else
                            go.GetComponent<MeshRenderer>().material = _topMaterial;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForEndOfFrame();
        PlaceableArea = maps[^1];
    }
    /// <summary>
    /// Returning a cube, Making sure to instantiate if there's no avaliable inactive cubes
    /// </summary>
    /// <param name="position"> The position to place the block</param>
    /// <returns>the spawned cube</returns>
    public GameObject GetCube(Vector3 position)
    {
        //check wether or not there's inactive cubes saved
        if (_inactiveCubes.Count > 0)
        {
            //Get a inactive cube to activate and place
            GameObject go = _inactiveCubes[0];
            go.transform.position = position;
            go.SetActive(true);
            _inactiveCubes.Remove(go);
            _activeCubes.Add(go);
            return go;
        }
        else
        {
            //Create and place a new cube
            GameObject go = Instantiate(_cubePrefab, position, Quaternion.identity, transform);
            _activeCubes.Add(go);
            return go;
        }
    }
    /// <summary>
    /// Adding cube to the inactive cube list, allowing to spawn them more easily and disabling the cube
    /// </summary>
    /// <param name="cube">The cube to remove</param>
    public void RemoveCube(GameObject cube)
    {
        //hide and save the cube
        _activeCubes.Remove(cube);
        cube.SetActive(false);
        _inactiveCubes.Add(cube);
    }
}
