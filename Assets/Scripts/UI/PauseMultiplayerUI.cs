using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameManager.Instance.OnMultiplayerGamePaused += (sender, args) => Show();
        KitchenGameManager.Instance.OnMultiplayerGameUnpaused += (sender, args) => Hide();

        Hide();
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
