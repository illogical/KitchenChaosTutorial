using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())     
        {
            // there is no object on the counter

            if (player.HasKitchenObject())
            {
                // player is carrying something
                // put the item on the empty counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
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
        if (HasKitchenObject())
        {
            // there is a kitchen object here so let's cut it
            // destroy object and replace it with the cut object
            KitchenObjectSO outputKitchenObject = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            GetKitchenObject().DestroySelf();

            SpawnKitchenObject(outputKitchenObject, this);
        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (var cuttingObjectSO in cuttingRecipeSOArray)
        {
            if (cuttingObjectSO.Input == kitchenObjectSO)
            {
                return cuttingObjectSO.Output;
            }
        }

        return null;
    }
}
