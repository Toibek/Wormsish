using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    internal IslandManager IslandGen;
    internal ObjectManager ObjectGen;
    internal PlayerManager PlayerManager;

    internal int Units;
    internal int Moves;
    internal int Specials;
    internal int PickupFreq;

    internal int Barrels;
    internal int Mines;
    internal int Healthpacks;


    private void Awake()
    {
        PlayerManager = GetComponent<PlayerManager>();
    }
    public void LoadIntoGame()
    {
        SceneManager.LoadScene(1);
    }
    public void StartGame()
    {
        StartCoroutine(PlayerManager.SpawnPlayers());
    }
}
