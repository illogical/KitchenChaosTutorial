using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeDelivered;
    
    
    [SerializeField] private float spawnRecipeTimerMax = 4f;
    [SerializeField] private int waitingRecipeMax = 4;
    [SerializeField] private RecipeListSO recipeList;
    
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;

    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSO = recipeList.RecipeSOList[UnityEngine.Random.Range(0, recipeList.RecipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);
                
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.KitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateContentsMatchesRecipe = true;
                // recipe has the same number of ingredients as that plate does so let's ensure the ingredients match
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.KitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    foreach(KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound)
                    {
                        // this recipe ingredient was not found on the plate
                        plateContentsMatchesRecipe = false;
                    }

                    if (plateContentsMatchesRecipe)
                    {
                        // player delivered the correct recipe!
                        waitingRecipeSOList.RemoveAt(i);
                        
                        OnRecipeDelivered?.Invoke(this, EventArgs.Empty);
                        return;
                    }
                }
            }
        }
        
        // no matches found- player delivered the wrong recipe
        Debug.Log("That is the wrong meal!");
    }

    public List<RecipeSO> GetWaitingRecipeListSO() => waitingRecipeSOList;
}
