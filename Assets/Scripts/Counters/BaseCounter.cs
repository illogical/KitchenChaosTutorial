using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedHere;
    public static void ResetStaticData() => OnAnyObjectPlacedHere = null;

    [SerializeField] protected Transform counterTopPoint;
   
    private KitchenObject kitchenObject;
   
    
    public virtual Transform GetKitchenObjectFollowTransform() => counterTopPoint;

    public virtual bool HasKitchenObject() => kitchenObject != null;

    public virtual KitchenObject GetKitchenObject() => kitchenObject;

    public NetworkObject GetNetworkObject() => NetworkObject;

    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.Interact() executed. Don't do that.");
    }

    public virtual void InteractAlternate(Player player)
    {
        //Debug.LogError("BaseCounter.InteractAlternate() executed. Don't do that.");
    }

    public virtual void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    public virtual void ClearKitchenObject()
    {
        kitchenObject = null;
    }

}
