using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    public Color Color = Color.black;
    private bool AvatarReceived;

    public TMP_Text PlayerNameText;
    public RawImage PlayerIcon;
    public TMP_Text PlayerReadyText;
    public Image PlayerSkin;
    public bool Ready;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    public void ChangeReadyStatus()
    {
        if (Ready)
        {
            PlayerReadyText.text = "Ready";
            PlayerReadyText.color = Color.green;
        }
        else
        {
            PlayerReadyText.text = "Unready";
            PlayerReadyText.color = Color.red;
        }
    }

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == PlayerSteamID)
        {
            PlayerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            return;
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return texture;
    }

    public void GetPlayerIcon()
    {
        if (AvatarReceived) return;

        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (ImageID == -1) return;
        PlayerIcon.texture = GetSteamImageAsTexture(ImageID);
    }

    public void CangeSkin()
    {
        PlayerSkin.color = Color;
    }

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        ChangeReadyStatus();
        CangeSkin();
        GetPlayerIcon();
    }
}
