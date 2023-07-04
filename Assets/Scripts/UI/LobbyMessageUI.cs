using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameMultiplayer_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameMultiplayer_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameMultiplayer_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameMultiplayer_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameMultiplayer_OnJoinFailed;
        Hide();
    }

    

    private void KitchenGameMultiplayer_OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMessage("Creating lobby...");
    }

    private void KitchenGameMultiplayer_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to create lobby");
    }

    private void KitchenGameMultiplayer_OnJoinStarted(object sender, EventArgs e)
    {
        ShowMessage("Joining lobby...");
    }

    private void KitchenGameMultiplayer_OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to find a lobby to quick join");
    }

    private void KitchenGameMultiplayer_OnJoinFailed(object sender, EventArgs e)
    {
        ShowMessage("Failed to join lobby");
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    public void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameMultiplayer_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameMultiplayer_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameMultiplayer_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameMultiplayer_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameMultiplayer_OnJoinFailed;
    }


    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
