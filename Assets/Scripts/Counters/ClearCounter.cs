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
            if (!player.HasKitchenObject())
            {
                // player is not carrying something   
                // pick up the object
                GetKitchenObject().SetKitchenObjectParent(player);
            }
           
        }
       
    }
}
