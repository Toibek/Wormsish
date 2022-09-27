using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerState PlayerState = PlayerState.Menu;
    internal IslandManager IslandGen;
    internal ObjectManager ObjectGen;
    internal PlayerManager PlayerManager;
    internal PlayfabManager PlayfabManager;

    string _username;
    string _islandString;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        IslandGen = GetComponent<IslandManager>();
        ObjectGen = GetComponent<ObjectManager>();
        PlayerManager = GetComponent<PlayerManager>();
        PlayfabManager = GetComponent<PlayfabManager>();
        PlayfabManager.OnDataRecieved += HandleData;
        PlayfabManager.OnChatRecieved += HandleData;
        _username = PlayfabManager.DisplayName;
    }
    public void LoadIntoGame()
    {
        SceneManager.LoadScene(1);
    }
    private void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            switch (PlayerState)
            {
                case PlayerState.Menu:
                    Debug.LogWarning("Loaded level without selecting state");
                    break;
                case PlayerState.Local:
                case PlayerState.Host:
                    _islandString = IslandGen.GenerateIsland();
                    break;
                case PlayerState.Remote:
                case PlayerState.Spectator:
                    PlayfabManager.SendDataMessage("R:intro");
                    break;
                default:
                    Debug.LogWarning("Inpossible playerstate");
                    break;
            }
        }
    }
    void HandleData(string data)
    {
        int typeIndex = data.IndexOf(':');
        string type;
        if (typeIndex >= 0)
            type = data.Substring(0, typeIndex);
        else
            Debug.LogWarning("Data did not have a currect type\n" + data);
    }
    public void StartGame()
    {
        StartCoroutine(PlayerManager.SpawnPlayers());
    }

}
public enum PlayerState { Menu, Local, Host, Remote, Spectator }
