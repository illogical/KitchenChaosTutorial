using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

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
            // counter contains an object
            
            if (player.HasKitchenObject())
            {
                // player is carrying something

                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) ;
                {
                    // player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
            }
            else
            {
                // player is not carrying something   
                // pick up the object
                GetKitchenObject().SetKitchenObjectParent(player);
            }
           
        }
       
    }
}
