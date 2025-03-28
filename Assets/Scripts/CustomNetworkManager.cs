using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
        GamePlayerInstance.ConnectionID = conn.connectionId;
        GamePlayerInstance.PlayerIdNumber = GamePlayers.Count + 1;
        GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.CurrentLobbyID, GamePlayers.Count);

        NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
    }
    public void ChangeScene(string SceneName)
    {
        ServerChangeScene(SceneName);
    }
}
