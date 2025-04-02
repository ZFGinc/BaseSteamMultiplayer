using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance;
    // Callbacks
    protected Callback<LobbyCreated_t> LobbyCreate;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdated;

    public List<CSteamID> lobbyIDs = new List<CSteamID>();

    //Vars
    public ulong CurrentLobbyID;
    private const string HostAddresKey = "HostAddres";
    private CustomNetworkManager manager;
    private bool IsHost = false;

    private void Start()
    {
        if (!SteamManager.Initialized) return;

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        manager = GetComponent<CustomNetworkManager>();

        LobbyCreate = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnterd);

        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
        LobbyDataUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        Debug.Log("Created Lobby");

        manager.StartHost();
        IsHost = true;

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddresKey,
            SteamUser.GetSteamID().ToString()
        );

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "name",
            "Host: " + SteamFriends.GetPersonaName().ToString()
        );
    }
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }
    private void OnLobbyEnterd(LobbyEnter_t callback)
    {
        //Eneryone
        CurrentLobbyID = callback.m_ulSteamIDLobby;

        //Clients
        if (NetworkServer.active) return;
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddresKey);
        manager.StartClient();
    }
    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }
    public void GetLobbiesList()
    {
        if (lobbyIDs.Count > 0) lobbyIDs.Clear();

        //SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.AddRequestLobbyListStringFilter("name", "Host: ", ELobbyComparison.k_ELobbyComparisonEqualToOrGreaterThan);
        SteamMatchmaking.RequestLobbyList();
    }
    public void LeaveLobby()
    {
        if (IsHost)
        {
            manager.StopHost();
            IsHost = false;
        }

        manager.StopClient();
        SteamMatchmaking.LeaveLobby(new CSteamID(CurrentLobbyID));
        CurrentLobbyID = 0;
        UIManager.Instance.OpenMenuCanvas();
        UIManager.Instance.DefaultMenu();
    }
    private void OnGetLobbyList(LobbyMatchList_t result)
    {
        if (LobbiesListManager.instance.listOfLobbies.Count > 0)
            LobbiesListManager.instance.DestroyLobbies();

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }
    private void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        LobbiesListManager.instance.DisplayLobbies(lobbyIDs, result);
    }


}
