using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerState PlayerState = PlayerState.Menu;
    public Dictionary<string, string> Players = new Dictionary<string, string>();

    internal IslandManager IslandGen;
    internal ObjectManager ObjectGen;
    internal PlayerManager PlayerManager;


    string _username;
    string _islandString;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        IslandGen = GetComponent<IslandManager>();
        ObjectGen = GetComponent<ObjectManager>();
        PlayerManager = GetComponent<PlayerManager>();
    }
    public void LoadIntoGame()
    {
        SceneManager.LoadScene(1);
    }
    void HandleChat(string data)
    {
        Debug.Log(data);
    }
    public void StartGame()
    {
        StartCoroutine(PlayerManager.SpawnPlayers());
    }

}
public enum PlayerState { Menu, Local, Host, Remote, Spectator }
