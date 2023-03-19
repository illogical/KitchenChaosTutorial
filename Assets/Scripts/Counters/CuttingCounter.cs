using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    public class OnProgressChangedEventArgs : EventArgs
    {
        public float ProgressNormalized;
    }
    
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;
    
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())     
        {
            // there is no object on the counter

            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                // player is carrying something
                // put the item on the empty counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
                cuttingProgress = 0;
                CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                OnProgressChanged.Invoke(this, new OnProgressChangedEventArgs
                {
                    ProgressNormalized = (float)cuttingProgress / cuttingRecipeSO.CuttingProgressMax
                });
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                // player is not carrying something   
                // pick up the object
                GetKitchenObject().SetKitchenObjectParent(player);
            }
           
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // there is a kitchen object that has a cutting recipe here so let's cut it
            // destroy object and replace it with the cut object
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);
            
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            KitchenObjectSO outputKitchenObject = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged.Invoke(this, new OnProgressChangedEventArgs
            {
                ProgressNormalized = (float)cuttingProgress / cuttingRecipeSO.CuttingProgressMax
            });
            
            if (cuttingProgress >= cuttingRecipeSO.CuttingProgressMax)
            {
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObject, this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        return GetCuttingRecipeSOWithInput(kitchenObjectSO) != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        return GetCuttingRecipeSOWithInput(kitchenObjectSO)?.Output;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (var cuttingObjectSO in cuttingRecipeSOArray)
        {
            if (cuttingObjectSO.Input == kitchenObjectSO)
            {
                return cuttingObjectSO;
            }
        }

        return null;
    }
}
