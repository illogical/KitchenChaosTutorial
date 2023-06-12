using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [FormerlySerializedAs("visualGameObject")] [SerializeField] private GameObject[] visualGameObjectArray;
    
    private void Start()
    {
        if(Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged += InstanceOnOnSelectedCounterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawned += PlayerOnOnAnyPlayerSpawned;
        }
       
    }

    private void PlayerOnOnAnyPlayerSpawned(object sender, EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            // since this could run multiple times, we need to unsubscribe from the event first
            Player.LocalInstance.OnSelectedCounterChanged -= InstanceOnOnSelectedCounterChanged;
            Player.LocalInstance.OnSelectedCounterChanged += InstanceOnOnSelectedCounterChanged;
        }
    }

    private void InstanceOnOnSelectedCounterChanged(object sender, Player.OnSelectedChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide(); 
        }
    }

    private void Show()
    {
        foreach (var visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
        
        
    }

    private void Hide()
    {
        foreach (var visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
