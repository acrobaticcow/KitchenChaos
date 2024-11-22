using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent
{
    public KitchenObject GetKitchenObject();

    public void ClearKitchenObject();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public bool HasKitchenObject();
    public Transform GetKitchenObjectFollowTransform();

    public NetworkObject GetNetWorkObject();
}
