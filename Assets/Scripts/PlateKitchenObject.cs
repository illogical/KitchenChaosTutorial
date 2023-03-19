using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    [SerializeField] private List<KitchenObjectSO> validKitchenSOList;
    
    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake()
    {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenSOList.Contains(kitchenObjectSO))
        {
            // not a valid ingredient
            return false;
        }
        
        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            // already contains this type
            return false;
        }
        
        kitchenObjectSOList.Add((kitchenObjectSO));
        return true;
    }
}
