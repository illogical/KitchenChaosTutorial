using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStartClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);

        // in case the game was restarted, unpause the game     
        Time.timeScale = 1f;
    }


    private void OnStartClicked()
    {
        Loader.Load(Loader.Scene.GameScene);
    }
    
    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
