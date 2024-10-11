using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientsAddedEventArgs> OnIngredientAdded;

    public class OnIngredientsAddedEventArgs : EventArgs
    {
        public KitchenObjectsSO kitchenObjectSO;
    }

    [SerializeField]
    private List<KitchenObjectsSO> validKitchenObjectSOList;
    private List<KitchenObjectsSO> kitchenObjectSOList;

    private void Awake()
    {
        kitchenObjectSOList = new();
    }

    public List<KitchenObjectsSO> GetKitchenObjectList()
    {
        return kitchenObjectSOList;
    }

    public bool TryAddIngredients(KitchenObjectsSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
            return false;

        if (kitchenObjectSOList.Contains(kitchenObjectSO))
            return false;

        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(
            this,
            new OnIngredientsAddedEventArgs { kitchenObjectSO = kitchenObjectSO }
        );
        return true;
    }
}
