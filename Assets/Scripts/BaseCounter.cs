using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] protected Transform counterTopPoint;
   

    private KitchenObject kitchenObject;
    
    public virtual void Interact(Player player)
    {
    }
    
    public virtual Transform GetKitchenObjectFollowTransform() => counterTopPoint;
    public virtual void SetKitchenObject(KitchenObject kitchenObject) => this.kitchenObject = kitchenObject;
    public virtual bool HasKitchenObject() => kitchenObject != null;

    public virtual KitchenObject GetKitchenObject() => kitchenObject;

    public virtual void ClearKitchenObject()
    {
        kitchenObject = null;
    }
}
