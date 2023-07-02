using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{

    [SerializeField] private int playerIndex;

    [SerializeField] private GameObject readyGameObject;

    [SerializeField] private PlayerVisual playerVisual;


    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if(KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            readyGameObject.SetActive(
                CharacterSelectReady.Instance.IsPlayerReady(
                    KitchenGameMultiplayer.Instance.GetPlayerData(playerIndex).ClientID));

            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerIndex));
        }
        else
        {
            Hide();
        }
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
