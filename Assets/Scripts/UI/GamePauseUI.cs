using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => KitchenGameManager.Instance.TogglePauseGame());
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown(); // shutdown any existing connection
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButton.onClick.AddListener(() => OptionsUI.Instance.Show());
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnLocalGamePaused += (sender, args) => Show();
        KitchenGameManager.Instance.OnLocalGameUnpaused += (sender, args) => Hide();

        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
