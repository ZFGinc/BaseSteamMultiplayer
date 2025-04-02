using Mirror;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    [Scene]
    public string MenuScene;
    [Scene]
    public string MultiplayerScene;

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

    public string GetSceneMultiplayer(string path)
    {
        string[] tmp = MultiplayerScene.Split('.')[0].Split('/');
        return tmp[tmp.Length - 1];
    }

    private void Update()
    {
        Debug.Log("Current: " + SceneManager.GetActiveScene().name);

        if (SceneManager.GetActiveScene().name == GetSceneMultiplayer(MultiplayerScene))
        {
            if (UIManager.Instance.IsMenu)
                UIManager.Instance.CloseMenuCanvas();
        }
        else
        {
            if (!UIManager.Instance.IsMenu)
                UIManager.Instance.OpenMenuCanvas();
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        ServerChangeScene(GetSceneMultiplayer(MenuScene));
    }
}
