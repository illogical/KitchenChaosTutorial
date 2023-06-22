using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : NetworkBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeDelivered;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    
    
    [SerializeField] private float spawnRecipeTimerMax = 4f;
    [SerializeField] private int waitingRecipeMax = 4;
    [SerializeField] private RecipeListSO recipeList;
    
    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private int successfulRecipeAmount;

    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
        spawnRecipeTimer = spawnRecipeTimerMax;
    }

    private void Update()
    {
        if(!IsServer) return;

        spawnRecipeTimer -= Time.deltaTime;

        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeList.RecipeSOList.Count);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }

    [ClientRpc]
    // keep in mind that this method is called on the server and then the server calls it on the clients
    public void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeSO waitingRecipeSO = recipeList.RecipeSOList[waitingRecipeSOIndex];

        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
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
                        DeliverCorrectRecipeServerRpc(i);
                        return;
                    }
                }
            }
        }

        // no matches found- player delivered the wrong recipe
        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex) => DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        successfulRecipeAmount++;
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeDelivered?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc() => DeliverIncorrectRecipeClientRpc();

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        Debug.Log("That is the wrong meal!");
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeListSO() => waitingRecipeSOList;
    public int GetSuccessfulRecipesAmount() => successfulRecipeAmount;
}
