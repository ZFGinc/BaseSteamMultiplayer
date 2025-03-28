using Steamworks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public TMP_Text LobbyNameText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;
    public GameObject LobbyMenu;
    public Image SkinPreview;

    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalplayerController;

    public Button StartGameButton;
    public TMP_Text ReadyButtonText;

    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null) return manager;
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ReadyPlayer()
    {
        LocalplayerController.ChangeReady();
    }
    public void UpdateButton()
    {
        if (LocalplayerController.Ready) ReadyButtonText.text = "Unready";
        else ReadyButtonText.text = "Ready";
    }
    public void ChangeSkinIndexLocal(int value)
    {
        if (LocalplayerController is null) return;
        if (value < -1 || value > 1 || value == 0) return;

        LocalplayerController.ChangeSkinIndexLocal(value);

        SkinPreview.color = LocalplayerController.Skins[LocalplayerController.PlayerSkin].color;
    }

    public void ChackIfAllReady()
    {
        bool AllReady = false;

        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (player.Ready) AllReady = true;
            else
            {
                AllReady = false;
                break;
            }
        }

        if (AllReady)
        {
            if (LocalplayerController.PlayerIdNumber == 1) StartGameButton.interactable = true;
            else StartGameButton.interactable = false;
        }
        else StartGameButton.interactable = false;
    }
    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }
    public void UpdatePlayerList()
    {
        UpdateLobbbyMenu();
        if (!PlayerItemCreated) CreateHostPlayerItem();
        if (PlayerListItems.Count < Manager.GamePlayers.Count) CreateClientPlayerItem();
        if (PlayerListItems.Count > Manager.GamePlayers.Count) RemovePlayerItem();
        if (PlayerListItems.Count == Manager.GamePlayers.Count) UpdatePlayerItem();
    }
    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalplayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }
    public void CreateHostPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.Color = player.Skins[player.PlayerSkin].color;
            NewPlayerItemScript.SetPlayerValues();

            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform, false);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);
        }
        PlayerItemCreated = true;
    }
    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.Color = player.Skins[player.PlayerSkin].color;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform, false);
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }
    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.ConnectionID)
                {
                    PlayerListItemScript.PlayerName = player.PlayerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.Color = player.Skins[player.PlayerSkin].color;
                    PlayerListItemScript.SetPlayerValues();

                    if (player == LocalplayerController) UpdateButton();
                }
            }
        }

        ChackIfAllReady();
    }
    public void RemovePlayerItem()
    {
        List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

        foreach (PlayerListItem playerListItem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(b => b.ConnectionID == playerListItem.ConnectionID))
            {
                playerListItemToRemove.Add(playerListItem);
            }
        }

        if (playerListItemToRemove.Count > 0)
        {
            foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
            {
                GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                PlayerListItems.Remove(playerlistItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
    }
    public void UpdateLobbbyMenu()
    {
        if (!LobbyMenu.activeSelf) LobbyMenu.SetActive(true);
    }
    public void StartGame(string SceneName)
    {
        LocalplayerController.CanStartGame(SceneName);
    }

    public void LeaveGame(string SceneName)
    {
        UIManager.Instance.DefaultMenu();
        LocalplayerController.CanStopGame(SceneName);
    }
}


