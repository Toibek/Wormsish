using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Doozy.Engine.UI;

public class MenuManager : MonoBehaviour
{
    string _username;
    [SerializeField] PlayfabManager _pfManager;
    [Header("Login")]
    [SerializeField] UIView LoginView;
    [SerializeField] TMP_InputField loginNameField;
    [Header("MainMenu")]
    [SerializeField] UIView MainView;
    [SerializeField] Button HostButton;
    [SerializeField] Button SocialButton;
    [Header("Friends")]
    [SerializeField] UIView FriendView;
    [SerializeField] Transform FriendHolder;
    [SerializeField] TMP_InputField NewFriendField;
    [SerializeField] Button PrefabFriendButton;
    private void Start()
    {
        _username = PlayerPrefs.GetString("username");
        if (!string.IsNullOrEmpty(_username))
            loginNameField.text = _username;
    }
    public void Login()
    {
        HostButton.interactable = true;
        SocialButton.interactable = true;
        LoginView.Hide();
        _username = loginNameField.text;
        PlayerPrefs.SetString("username", _username);
        _pfManager.Login(_username);
        _pfManager.OnLoginComplete += OnLoginComplete;
    }
    void OnLoginComplete()
    {
        _pfManager.OnLoginComplete -= OnLoginComplete;
        MainView.Show();
    }
    public void Offline()
    {
        HostButton.interactable = false;
        SocialButton.interactable = false;
        LoginView.Hide();
        MainView.Show();
    }
    public void ToggleFriends()
    {
        if (FriendView.IsActive())
            HideFriends();
        else
            ShowFriends();
    }
    void ShowFriends()
    {
        FriendView.Show();
        _pfManager.GetFriendsList();
        _pfManager.OnFriendsFetched += LoadFriends;
    }
    public void LoadFriends(string[] friends)
    {
        for (int i = FriendHolder.childCount - 1; i >= 0; i--)
            Destroy(FriendHolder.GetChild(i).gameObject);

        for (int i = 0; i < friends.Length; i++)
        {
            Button but = Instantiate(PrefabFriendButton, FriendHolder);
            but.name = friends[i];
            but.GetComponentInChildren<TMP_Text>().text = friends[i];
            but.onClick.AddListener(() => _pfManager.JoinFriend(but.name));
        }
    }
    public void AddFriend()
    {
        _pfManager.AddFriend(NewFriendField.text);
    }
    void HideFriends()
    {
        FriendView.Hide();
        _pfManager.OnFriendsFetched -= LoadFriends;
    }
    public void HostGame()
    {
        _pfManager.CreateParty();
    }
}
