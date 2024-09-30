using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    [SerializeField]
    private List<KitchenObjectsSO> validKitchenObjectSOList;
    private List<KitchenObjectsSO> kitchenObjectSOList;

    private void Awake()
    {
        kitchenObjectSOList = new();
    }

    public bool TryAddIngredients(KitchenObjectsSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
            return false;

        if (kitchenObjectSOList.Contains(kitchenObjectSO))
            return false;

        kitchenObjectSOList.Add(kitchenObjectSO);
        return true;
    }
}
