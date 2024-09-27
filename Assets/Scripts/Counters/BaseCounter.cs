using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField]
    private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public virtual void Interact(Player player)
    {
        Debug.LogError("BaseCounter.cs/Interact");
    }

    public virtual void InteractAlternate(Player player) { }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
