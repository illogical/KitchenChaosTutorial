using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Awake()
    {
        createGameButton.onClick.AddListener(OnCreateGameButtonClicked);
        joinGameButton.onClick.AddListener(OnJoinGameButtonClicked);
    }

    private void OnCreateGameButtonClicked()
    {
        KitchenGameMultiplayer.Instance.StartHost();
    }

    private void OnJoinGameButtonClicked()
    {
        KitchenGameMultiplayer.Instance.StartClient();
    }
}
