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

    private void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
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
