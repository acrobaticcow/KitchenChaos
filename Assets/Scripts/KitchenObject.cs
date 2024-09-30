using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField]
    private KitchenObjectsSO KitchenObjectsSO;

    private IKitchenObjectParent kitchenObjectParent;
    private float currentActionProgress;

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

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        this.kitchenObjectParent?.ClearKitchenObject();

        this.kitchenObjectParent = kitchenObjectParent;
        if (kitchenObjectParent.HasKitchenObject())
            Debug.LogError("This clearCounter already have a kitchen object");
        kitchenObjectParent.SetKitchenObject(this);

        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public static KitchenObject SpawnKitchenObject(
        KitchenObjectsSO kitchenObjectSO,
        IKitchenObjectParent kitchenObjectParent
    )
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.Prefab);
        var kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

        return kitchenObject;
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
