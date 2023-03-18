using UnityEngine;
using UnityEngine.Serialization;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [FormerlySerializedAs("visualGameObject")] [SerializeField] private GameObject[] visualGameObjectArray;
    
    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += InstanceOnOnSelectedCounterChanged;
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
