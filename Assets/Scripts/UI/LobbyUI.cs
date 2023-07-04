using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{

    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private CreateLobbyUI createLobbyUI;
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_InputField playerNameInput;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        createLobbyButton.onClick.AddListener(() =>
        {
            createLobbyUI.Show();
        });

        quickJoinButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.JoinWithCode(joinCodeInput.text);
        });
    }

    private void Start()
    {
        playerNameInput.text = KitchenGameMultiplayer.Instance.GetPlayerName();

        playerNameInput.onValueChanged.AddListener((string newText) =>
        {
            KitchenGameMultiplayer.Instance.SetPlayerName(newText);
        });
    }
}
