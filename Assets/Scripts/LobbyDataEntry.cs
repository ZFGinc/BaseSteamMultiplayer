using Steamworks;
using TMPro;
using UnityEngine;

public class LobbyDataEntry : MonoBehaviour
{
    public CSteamID lobbyID;
    public string lobbyName;
    public TMP_Text lobbyNameText;

    public void SetLobbyData()
    {
        if (lobbyName == "") lobbyNameText.text = "No name";
        else lobbyNameText.text = lobbyName;
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(lobbyID);
    }
}
