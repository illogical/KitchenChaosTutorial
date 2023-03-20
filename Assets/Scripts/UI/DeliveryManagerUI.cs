using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManagerOnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeDelivered += DeliverManagerOnRecipeDelivered;
        
        UpdateVisual();
    }

    private void DeliveryManagerOnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
        
    }
    
    private void DeliverManagerOnRecipeDelivered(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            // reset
            if (child != recipeTemplate)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeListSO())
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }
}
