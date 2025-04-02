using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectController : NetworkBehaviour
{
    //Player Data
    [SyncVar] public int ConnectionID;
    [SyncVar] public int PlayerIdNumber;
    [SyncVar] public ulong PlayerSteamID;
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string PlayerName;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool Ready;
    [SyncVar(hook = nameof(PlayerSkinUpdate))] public int PlayerSkin;

    public List<Material> Skins = new();
    public MeshRenderer PlayerSkinMesh;

    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null) return manager;
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer) this.Ready = newValue;
        if (isClient) LobbyController.Instance.UpdatePlayerList();
    }
    public void PlayerNameUpdate(string OldValue, string NewValue)
    {
        if (isServer) this.PlayerName = NewValue;
        if (isClient) LobbyController.Instance.UpdatePlayerList();
    }
    public void PlayerSkinUpdate(int OldValue, int NewValue)
    {
        if (isServer) this.PlayerSkin = NewValue;
        if (isClient) LobbyController.Instance.UpdatePlayerList();
    }
    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        LobbyController.Instance.FindLocalPlayer();
        LobbyController.Instance.UpdateLobbyName();
    }
    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
    }
    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);
        LobbyController.Instance.UpdatePlayerList();
    }
    public void ChangeReady()
    {
        if (hasAuthority) CmdSetPlayerReady();
    }
    public void ChangeSkinIndexLocal(int value)
    {
        if (hasAuthority)
        {
            if (value < -1 || value > 1 || value == 0) return;

            if (PlayerSkin + value < 0) PlayerSkin = Skins.Count - 1;
            else if (PlayerSkin + value >= Skins.Count) PlayerSkin = 0;
            else PlayerSkin += value;

            PlayerSkinMesh.material = Skins[PlayerSkin];
            CmdSetPlayerSkin(PlayerSkin);
        }
    }

    public void CanStartGame(string SceneName)
    {
        if (hasAuthority)
        {
            CmdCanStartGame(SceneName);
        }
    }
    public void CanStopGame(string SceneName)
    {
        if (hasAuthority)
        {
            CmdCanStopGame(SceneName);
        }
    }

    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        Debug.Log("CMD Start");
        manager.ChangeScene(SceneName);
    }
    [Command]
    public void CmdCanStopGame(string SceneName)
    {
        Debug.Log("CMD Stop");
        manager.ChangeScene(SceneName);
    }
    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        Debug.Log("Change Name");
        this.PlayerNameUpdate(this.PlayerName, playerName);
    }
    [Command]
    private void CmdSetPlayerSkin(int playerSkin)
    {
        Debug.Log("Change Skin");
        this.PlayerSkinUpdate(this.PlayerSkin, playerSkin);
        CmdSetPlayerReady();
        CmdSetPlayerReady();
    }
    [Command]
    private void CmdSetPlayerReady()
    {
        Debug.Log("Change Ready");
        this.PlayerReadyUpdate(this.Ready, !this.Ready);
    }
}
