using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public IslandGen IslandGen;
    public ObjectGen ObjectGen;
    public PlayerManager PlayerManager;

    public GameObject SetupUI;
    private void Start()
    {
        StartCoroutine(MapSpawn());
    }
    IEnumerator MapSpawn()
    {
        yield return StartCoroutine(IslandGen.GenerationRoutine());
        bool[,] placeable = IslandGen.PlaceableArea;
        yield return StartCoroutine(ObjectGen.GenerateObjects(placeable));
    }
    public void StartGame()
    {
        SetupUI.SetActive(false);
        StartCoroutine(PlayerManager.SpawnPlayers());
    }
}
