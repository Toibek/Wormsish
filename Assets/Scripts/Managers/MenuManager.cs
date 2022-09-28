using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] View _loginView;
    [SerializeField] TMP_InputField _loginNameField;
    [Header("MainMenu")]
    [SerializeField] View _mainView;
    [SerializeField] Button _hostButton;
    [SerializeField] Button _socialButton;
    [Header("Friends")]
    [SerializeField] View _friendView;
    [SerializeField] Transform _friendHolder;
    [SerializeField] TMP_InputField _newFriendField;
    [SerializeField] Button _prefabFriendButton;

    GameManager _gameManager;
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void Login()
    {
        _hostButton.interactable = true;
        _socialButton.interactable = true;
        _loginView.Hide();
    }
    void OnLoginComplete()
    {
        _mainView.Show();
    }
    public void Offline()
    {
        _hostButton.interactable = false;
        _socialButton.interactable = false;
        _loginView.Hide();
        _mainView.Show();
    }
    public void ToggleFriends()
    {
        if (_friendView.gameObject.activeInHierarchy)
            HideFriends();
        else
            ShowFriends();
    }
    void ShowFriends()
    {
        _friendView.Show();
    }
    public void LoadFriends(string[] friends)
    {
        for (int i = _friendHolder.childCount - 1; i >= 0; i--)
            Destroy(_friendHolder.GetChild(i).gameObject);

        for (int i = 0; i < friends.Length; i++)
        {
            Button but = Instantiate(_prefabFriendButton, _friendHolder);
            but.name = friends[i];
            but.GetComponentInChildren<TMP_Text>().text = friends[i];
            but.onClick.AddListener(() => JoinFriend(but.name));
        }
    }
    public void JoinFriend(string name)
    {
        _gameManager.PlayerState = PlayerState.Remote;
    }
    public void AddFriend()
    {
    }
    void HideFriends()
    {
        _friendView.Hide();
    }
    public void LocalGame()
    {
        _gameManager.PlayerState = PlayerState.Local;
    }
    public void HostGame()
    {
        _gameManager.PlayerState = PlayerState.Host;
    }
    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
