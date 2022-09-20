using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.MultiplayerModels;
using PlayFab.ClientModels;
using TMPro;
public class PlayfabManager : MonoBehaviour
{
    public string ConnectionString;
    PlayFabAuthenticationContext _authCont;
    EntityTokenResponse _key;
    [SerializeField] string OwnedLobbyId;
    [SerializeField] Dictionary<string, string> _memberData;
    [Space]
    [SerializeField] Transform LogContent;
    [SerializeField] GameObject LogText;
    [Space]
    [SerializeField] TMP_InputField CreateOutput;
    [Space]
    [SerializeField] TMP_InputField JoinInput;


    // Start is called before the first frame update
    void Start()
    {
        var request = new LoginWithCustomIDRequest { CustomId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnPlayfabError);
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(OwnedLobbyId))
        {
            DestroyLobby();
        }
    }
    PlayFab.MultiplayerModels.EntityKey MyKey()
    {
        return new PlayFab.MultiplayerModels.EntityKey
        {
            Id = _authCont.EntityId,
            Type = _authCont.EntityType
        };
    }
    public void WriteLog(string log)
    {
        GameObject go = Instantiate(LogText, LogContent);
        go.GetComponent<TMP_Text>().text = log;
    }
    void OnLoginSuccess(LoginResult result)
    {
        _authCont = result.AuthenticationContext;
        WriteLog("Logged in: " + result.PlayFabId);
    }
    public void QueueForMatch()
    {

    }
    public void CreateLobby()
    {
        var request = new CreateLobbyRequest { };
        request.Owner = MyKey();
        request.AccessPolicy = AccessPolicy.Public;
        request.MaxPlayers = 4;
        request.Members = new List<Member>();

        Member m = new();
        m.MemberEntity = MyKey();
        m.MemberData = _memberData;
        request.Members.Add(m);

        PlayFabMultiplayerAPI.CreateLobby(request, OnLobbyCreated, OnPlayfabError);
    }
    public void JoinLobby() { JoinLobby(JoinInput.text); }
    void JoinLobby(string connectionString)
    {
        var request = new JoinLobbyRequest { };
        request.ConnectionString = connectionString;
        request.AuthenticationContext = _authCont;
        request.MemberEntity = MyKey();
        request.MemberData = _memberData;
        PlayFabMultiplayerAPI.JoinLobby(request, OnLobbyJoined, OnPlayfabError);
    }
    public void FindLobbies()
    {
        var request = new FindLobbiesRequest();
        request.AuthenticationContext = _authCont;
        PlayFabMultiplayerAPI.FindLobbies(request, OnLobbiesFound, OnPlayfabError);
    }
    public void DestroyLobby()
    {
        var request = new DeleteLobbyRequest { };
        request.AuthenticationContext = _authCont;
        request.LobbyId = OwnedLobbyId;
        PlayFabMultiplayerAPI.DeleteLobby(request, OnLobbyDeleted, OnPlayfabError);
    }
    void OnLobbyCreated(CreateLobbyResult result)
    {
        OwnedLobbyId = result.LobbyId;
        ConnectionString = result.ConnectionString;
        if (CreateOutput != null)
            CreateOutput.text = result.ConnectionString;
        WriteLog("Lobby Created: " + result.LobbyId);
    }
    void OnLobbyJoined(JoinLobbyResult result)
    {
        WriteLog("Lobby Joined: " + result.LobbyId);
    }
    void OnLobbiesFound(FindLobbiesResult result)
    {
        for (int i = 0; i < result.Lobbies.Count; i++)
        {
            WriteLog(result.Lobbies[i].ConnectionString);
        }
    }
    void OnLobbyDeleted(LobbyEmptyResult result)
    {
        WriteLog("Lobby deleted");
    }
    void OnPlayfabError(PlayFabError error)
    {
        WriteLog("Playfab error");
        Debug.LogWarning(error);
    }

}
