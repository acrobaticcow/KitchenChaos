using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField]
    private KitchenObjectsSO KitchenObjectsSO;

    private IKitchenObjectParent kitchenObjectParent;
    private float currentActionProgress;
    private FollowTransform followTransform;

    public float CurrentActionProgress
    {
        get => currentActionProgress;
        set => currentActionProgress = value;
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public KitchenObjectsSO GetKitchenObjectSO()
    {
        return KitchenObjectsSO;
    }

    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetWorkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNOR)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNOR);
    }

    [ClientRpc]
    public void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNOR)
    {
        kitchenObjectParentNOR.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        var kitchenObjectParent =
            kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        this.kitchenObjectParent?.ClearKitchenObject();

        this.kitchenObjectParent = kitchenObjectParent;
        if (kitchenObjectParent.HasKitchenObject())
            Debug.LogError("This clearCounter already have a kitchen object");
        kitchenObjectParent.SetKitchenObject(this);

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnParent()
    {
        kitchenObjectParent.ClearKitchenObject();
    }

    public static void SpawnKitchenObject(
        KitchenObjectsSO kitchenObjectSO,
        IKitchenObjectParent kitchenObjectParent
    )
    {
        KitchenGameObjectMultiplayer.Instance.SpawnKitchenObject(
            kitchenObjectSO,
            kitchenObjectParent
        );
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        KitchenGameObjectMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }

    public class ProgressChangedEventArgs : EventArgs
    {
        public float ProgressNormalized { get; }

        public ProgressChangedEventArgs(float progressNormalized)
        {
            ProgressNormalized = progressNormalized;
        }
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }

        plateKitchenObject = null;
        return false;
    }
}
