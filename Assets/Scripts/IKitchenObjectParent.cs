using UnityEngine;

public interface IKitchenObjectParent
{
    public Transform GetKitchenObjectFollowTransform();
    public void SetKitchenObject(KitchenObject kitchenObject);
    public bool HasKitchenObject();
    public KitchenObject GetKitchenObject();
    public void ClearKitchenObject();
}
