using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerState PlayerState = PlayerState.Menu;
    public IslandManager IslandGen;
    public ObjectManager ObjectGen;
    public PlayerManager PlayerManager;

    public GameObject SetupUI;
    private void Start()
    {

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
public enum PlayerState { Menu, Single, Host, Remote, Spectator }
