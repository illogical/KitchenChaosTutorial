using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

// Odd that a Monobehaviour is used for this
public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;

        playAgainButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown(); // shutdown any existing connection
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        Hide();
    }

    private void NetworkManagerOnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == NetworkManager.ServerClientId)
        {
            // server is shutting down
            Show();
        }
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
