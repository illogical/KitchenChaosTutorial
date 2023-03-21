using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyObjectPlacedHere;
    
    [SerializeField] protected Transform counterTopPoint;
   

    private KitchenObject kitchenObject;
    
    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.Interact() executed. Don't do that.");
    }
    
    public virtual void InteractAlternate(Player player)
    {
        //Debug.LogError("BaseCounter.InteractAlternate() executed. Don't do that.");
    }
    
    public virtual Transform GetKitchenObjectFollowTransform() => counterTopPoint;

    public virtual void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    } 
    public virtual bool HasKitchenObject() => kitchenObject != null;

    public virtual KitchenObject GetKitchenObject() => kitchenObject;

    public virtual void ClearKitchenObject()
    {
        kitchenObject = null;
    }
}
