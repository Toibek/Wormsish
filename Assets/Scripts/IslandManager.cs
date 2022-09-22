using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    /*
     For future reference-
    If there's anything missing from this it's to optimize the cubes themselves in order to make it take up less
    rendering time. seems to work perfectly fine with the current setup so currently set on the backburner.
     */

    internal bool[,] PlaceableArea;
    [Header("Generation settings")]
    [SerializeField] private int _cubesToPreload = 5000;
    [SerializeField] private int Size = 75;
    [SerializeField] private int Layers = 3;
    [SerializeField] [Range(0, 100)] private int NoiseDensity = 86;
    [SerializeField] [Range(1, 7)] private int Neighbors = 5;
    [SerializeField] [Range(0, 20)] private int Iterations = 6;
    [Header("Cube settings")]
    [SerializeField] GameObject _cubePrefab;
    [SerializeField] Material _topMaterial;
    [SerializeField] Material _botMaterial;

    List<GameObject> _inactiveCubes = new List<GameObject>();
    List<GameObject> _activeCubes = new List<GameObject>();

    GameObject[,,] _activeMapObjects;
    bool[,,] _activeMap;

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
    public IEnumerator GenerationRoutine(int seed = -1, bool[,,] preGenerated = null)
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
        //List<bool[,]> maps = new List<bool[,]>();
        bool[,,] maps = Cellular.GenerateArray(seed, Layers, Size, NoiseDensity, Neighbors, Iterations);
        //Place the cubes in the enabled positions
        Vector3Int offset = new(-maps.GetLength(0) / 2, 0, -maps.GetLength(2) / 2);
        PlaceableArea = new bool[maps.GetLength(0), maps.GetLength(2)];
        for (int y = 0; y < maps.GetLength(1); y++)
        {
            for (int x = 0; x < maps.GetLength(0) - y; x++)
            {
                for (int z = 0; z < maps.GetLength(2) - z; z++)
                {
                    if (maps[x, y, z])
                    {
                        GameObject go = GetCube(new Vector3(x, z, z) + offset);
                        if (y < maps.GetLength(1) - 1 && maps[x, y + 1, z])
                            go.GetComponent<MeshRenderer>().material = _botMaterial;
                        else
                        {
                            go.GetComponent<MeshRenderer>().material = _topMaterial;
                            PlaceableArea[x, z] = true;
                        }
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForEndOfFrame();
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
