using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingController : MonoBehaviour
{
    [SerializeField] private IslandGen _islandGen;
    [SerializeField] private ObjectGen _objectGen;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private GameObject _explosionPrefab;
    private void Start()
    {
        StartCoroutine(MapSpawn());
    }
    IEnumerator MapSpawn()
    {
        yield return StartCoroutine(_islandGen.GenerationRoutine());
        bool[,] placeable = _islandGen.PlaceableArea;
        yield return StartCoroutine(_objectGen.GenerateObjects(placeable));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(_playerManager.SpawnPlayers());
        Debug.Log("Generation complete");
    }
}
