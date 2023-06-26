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

                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject playerPlateKitchenObject))
                {
                    // player is holding a plate
                    if (playerPlateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenGameMultiplayer.Instance.DestroyKitchenObject(GetKitchenObject());
                    }
                }
                else
                {
                    // player is carrying something other than a plate
                    if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                    {
                        // plate is on the counter
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            KitchenGameMultiplayer.Instance.DestroyKitchenObject(player.GetKitchenObject());
                        }
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
