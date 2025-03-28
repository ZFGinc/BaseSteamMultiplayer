using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;

    public GameObject lobbiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void DestroyLobbies()
    {
        foreach (GameObject item in listOfLobbies)
        {
            Destroy(item);
        }

        listOfLobbies.Clear();
    }
    public void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createItem = Instantiate(lobbyDataItemPrefab);
                createItem.GetComponent<LobbyDataEntry>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;

                createItem.GetComponent<LobbyDataEntry>().lobbyName =
                    SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");

                createItem.GetComponent<LobbyDataEntry>().SetLobbyData();

                createItem.transform.SetParent(lobbyListContent.transform, false);
                createItem.transform.localScale = Vector3.one;

                listOfLobbies.Add(createItem);
            }
        }
    }
    public void GetListOfLobbies()
    {
        lobbiesMenu.SetActive(true);

        SteamLobby.Instance.GetLobbiesList();
    }
}
