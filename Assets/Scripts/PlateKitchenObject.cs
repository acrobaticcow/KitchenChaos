using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    protected override void Awake()
    {
        base.Awake();
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

        AddIngredientServerRpc(
            KitchenGameObjectMultiplayer.Instance.GetKitchenObjectSoIndex(kitchenObjectSO)
        );
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        var kitchenObjectSO = KitchenGameObjectMultiplayer.Instance.GetKitchenObjectSOFromIndex(
            kitchenObjectSOIndex
        );
        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(
            this,
            new OnIngredientsAddedEventArgs { kitchenObjectSO = kitchenObjectSO }
        );
    }
}
