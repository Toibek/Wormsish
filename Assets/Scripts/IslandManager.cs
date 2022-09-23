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
    [SerializeField] [Range(0, 20)] private int _baseIterations = 4;
    [SerializeField] [Range(0, 20)] private int _taperingIterations = 6;
    [Header("Cube settings")]
    [SerializeField] GameObject _cubePrefab;
    [SerializeField] Material _topMaterial;
    [SerializeField] Material _botMaterial;

    private List<GameObject> _inactiveCubes = new List<GameObject>();

    GameObject[,,] _activeMapObjects;
    bool[,,] _activeMap;
    Vector3 _mapOffset;
    private Coroutine _preLoadRoutine;
    private void Start()
    {
        _preLoadRoutine = StartCoroutine(PreloadCubes());
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
    public void ClearMap()
    {
        //Remove active cubes
        if (_activeMapObjects != null)
            for (int y = _activeMapObjects.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < _activeMapObjects.GetLength(0); x++)
                    for (int z = 0; z < _activeMapObjects.GetLength(2); z++)
                        if (_activeMapObjects[x, y, z] != null)
                            RemoveCube(_activeMapObjects[x, y, z]);
            }
    }
    /// <summary>
    /// A coroutine to create the base level, allowing for breaks in the creation.
    /// </summary>
    public IEnumerator GenerationRoutine(int seed = -1, bool[,,] preGenerated = null)
    {
        //Stop preloading if it's active
        if (_preLoadRoutine != null)
        {
            StopCoroutine(_preLoadRoutine);
            _preLoadRoutine = null;
        }

        //Place the cubes in the enabled positions
        _activeMap = Cellular.GenerateArray(seed, Layers, Size, NoiseDensity, Neighbors, _baseIterations, _taperingIterations);
        _activeMapObjects = new GameObject[_activeMap.GetLength(0), _activeMap.GetLength(1), _activeMap.GetLength(2)];
        _mapOffset = new(-_activeMap.GetLength(0) / 2, 0, -_activeMap.GetLength(2) / 2);
        PlaceableArea = new bool[_activeMap.GetLength(0), _activeMap.GetLength(2)];
        for (int y = 0; y < _activeMap.GetLength(1); y++)
        {
            for (int x = 0; x < _activeMap.GetLength(0); x++)
            {
                for (int z = 0; z < _activeMap.GetLength(2); z++)
                {
                    if (_activeMap[x, y, z])
                    {
                        GameObject go = GetCube(new Vector3(x, y, z) + _mapOffset);
                        _activeMapObjects[x, y, z] = go;
                        if (y < _activeMap.GetLength(1) - 1 && _activeMap[x, y + 1, z])
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
            return go;
        }
        else
        {
            //Create and place a new cube
            GameObject go = Instantiate(_cubePrefab, position, Quaternion.identity, transform);
            return go;
        }
    }
    /// <summary>
    /// Adding cube to the inactive cube list, allowing to spawn them more easily and disabling the cube
    /// </summary>
    /// <param name="cube">The cube to remove</param>
    public void RemoveCube(GameObject cube)
    {
        Vector3 pos = cube.transform.position - _mapOffset;
        //hide and save the cube
        _activeMapObjects[(int)pos.x, (int)pos.y, (int)pos.z] = null;
        _activeMap[(int)pos.x, (int)pos.y, (int)pos.z] = false;
        cube.SetActive(false);
        _inactiveCubes.Add(cube);
    }
}
