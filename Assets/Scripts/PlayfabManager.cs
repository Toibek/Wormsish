using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.MultiplayerModels;
using PlayFab.ClientModels;
using PlayFab.Party;
using TMPro;
using UnityEngine.UI;
public class PlayfabManager : MonoBehaviour
{
    public string PartyString;
    public string DisplayName;
    public string EnteredName;
    PlayFabAuthenticationContext _authCont;
    List<FriendInfo> _friends;

    private void OnApplicationQuit()
    {

        if (PlayFabMultiplayerManager.Get().State == PlayFabMultiplayerManagerState.ConnectedToNetwork)
            LeaveParty();
        RemovePersonalData("activeGame");
    }
    #region ClientStuff
    public void Login(string nameEntered)
    {
        EnteredName = nameEntered;
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnPlayfabError);
    }
    void OnLoginSuccess(LoginResult result)
    {
        _authCont = result.AuthenticationContext;
        Debug.Log("Logged in: " + result.PlayFabId);
        GetName();
        PlayFabMultiplayerManager.Get().OnError += OnPartyError;
    }
    void GetName()
    {
        var request = new GetAccountInfoRequest()
        {
            AuthenticationContext = _authCont
        };
        PlayFabClientAPI.GetAccountInfo(request, OnRetrievedName, OnPlayfabError);
    }
    void OnRetrievedName(GetAccountInfoResult result)
    {
        DisplayName = result.AccountInfo.TitleInfo.DisplayName;
        if (string.IsNullOrEmpty(EnteredName) && DisplayName != EnteredName) ChangeName(name);
        Debug.Log("Got Name: " + DisplayName);
    }

    public void ChangeName(string name)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name,
            AuthenticationContext = _authCont
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnChangeName, OnPlayfabError);
    }
    void OnChangeName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Name changed to: " + result.DisplayName);
    }

    public void AddFriend(string name)
    {
        var request = new AddFriendRequest();
        request.AuthenticationContext = _authCont;
        request.FriendTitleDisplayName = name;
        PlayFabClientAPI.AddFriend(request, OnFriendAdded, OnPlayfabError);
    }
    void OnFriendAdded(AddFriendResult result)
    {
        if (result.Created)
            Debug.Log("Friend added");
    }

    public void GetFriendsList()
    {
        var request = new GetFriendsListRequest
        {
            AuthenticationContext = _authCont,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowLastLogin = true
            }
        };
        PlayFabClientAPI.GetFriendsList(request, OnRetrievedFriendsList, OnPlayfabError);
    }
    void OnRetrievedFriendsList(GetFriendsListResult result)
    {
        _friends = result.Friends;
        for (int i = 0; i < _friends.Count; i++)
        {
            Debug.Log(_friends[i].TitleDisplayName + _friends[i].Profile.LastLogin);
        }
    }

    public void SetPersonalData(string key, string value)
    {
        var request = new UpdateUserDataRequest
        {
            AuthenticationContext = _authCont,
            Data = new Dictionary<string, string> { { key, value } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, DataSet, OnPlayfabError);
    }
    public void RemovePersonalData(string key)
    {
        var request = new UpdateUserDataRequest
        {
            AuthenticationContext = _authCont,
            KeysToRemove = new List<string> { key }
        };
        PlayFabClientAPI.UpdateUserData(request, DataSet, OnPlayfabError);
    }
    void DataSet(UpdateUserDataResult result)
    {
        Debug.Log("Data Updated");
    }
    public void GetUserData(string name, string data)
    {
        for (int i = 0; i < _friends.Count; i++)
        {
            if (_friends[i].TitleDisplayName == name)
            {
                var request = new GetUserDataRequest
                {
                    AuthenticationContext = _authCont,
                    PlayFabId = _friends[i].FriendPlayFabId,
                    Keys = new List<string>() { data }
                };
                PlayFabClientAPI.GetUserData(request, RetrievedUserData, OnPlayfabError);
            }
        }
    }
    void RetrievedUserData(GetUserDataResult result)
    {
        if (result.Data.ContainsKey("activeGame") && !string.IsNullOrEmpty(result.Data["activeGame"].Value))
        {
            JoinParty(result.Data["activeGame"].Value);
        }
        else
            Debug.Log("Friend is not currently in a game");
    }

    void OnPlayfabError(PlayFabError error)
    {
        Debug.Log("Playfab error: " + error.ErrorMessage);
        Debug.LogWarning(error);
    }
    #endregion

















    #region Party System
    public void CreateParty()
    {
        PlayFabMultiplayerManager.Get().CreateAndJoinNetwork();
        PlayFabMultiplayerManager.Get().OnNetworkJoined += OnPartyJoined;
    }
    public void JoinParty(string netId)
    {
        PlayFabMultiplayerManager.Get().JoinNetwork(netId);
        PlayFabMultiplayerManager.Get().OnNetworkJoined += OnPartyJoined;
    }
    public void LeaveParty()
    {
        PlayFabMultiplayerManager.Get().LeaveNetwork();
        PlayFabMultiplayerManager.Get().OnNetworkLeft += OnPartyLeft;
    }
    private void OnPartyJoined(object sender, string networkId)
    {
        Debug.Log("Party joined");
        PlayFabMultiplayerManager.Get().OnRemotePlayerJoined += OnRemotePlayerJoined;
        PlayFabMultiplayerManager.Get().OnChatMessageReceived += OnChatMessageReceived;
        PlayFabMultiplayerManager.Get().OnDataMessageReceived += OnDataMessageReceived;
        PlayFabMultiplayerManager.Get().OnRemotePlayerLeft += OnRemotePlayerLeft;

        SetPersonalData("activeGame", networkId);
    }

    public void SendChatMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log(_authCont.EntityId + " : " + message);
            PlayFabMultiplayerManager.Get().SendChatMessageToAllPlayers(message);
        }
    }
    public void SendDataMessage(byte[] buffer)
    {
        PlayFabMultiplayerManager.Get().SendDataMessageToAllPlayers(buffer);
    }
    private void OnRemotePlayerLeft(object sender, PlayFabPlayer player)
    {
        Debug.Log(player._entityToken + " Has left the party");
    }

    private void OnChatMessageReceived(object sender, PlayFabPlayer from, string message, ChatMessageType type)
    {
        Debug.Log(from._entityToken + " : " + message);
    }

    private void OnRemotePlayerJoined(object sender, PlayFabPlayer player)
    {
        Debug.Log(player._entityToken + " Has joined the party");
    }

    private void OnPartyLeft(object sender, string networkId)
    {
        Debug.Log("You have left the party");
    }
    private void OnDataMessageReceived(object sender, PlayFabPlayer from, byte[] buffer)
    {
        string message = "Datamessage:" + from + ":";
        for (int i = 0; i < buffer.Length; i++)
            message += buffer[i];
        Debug.Log(message);
    }


    private void OnPartyError(object sender, PlayFabMultiplayerManagerErrorArgs args)
    {
        Debug.LogError("Party error occured: " + args.Message);
    }
    #endregion


}
