using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public KitchenObjectsSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField]
    private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjectList;

    [SerializeField]
    private PlateKitchenObject plateKitchenObject;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        foreach (
            KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList
        )
        {
            kitchenObjectSOGameObject.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(
        object sender,
        PlateKitchenObject.OnIngredientsAddedEventArgs e
    )
    {
        foreach (
            KitchenObjectSO_GameObject kitchenObjectSOGameObject in kitchenObjectSOGameObjectList
        )
        {
            if (kitchenObjectSOGameObject.kitchenObjectSO == e.kitchenObjectSO)
            {
                kitchenObjectSOGameObject.gameObject.SetActive(true);
            }
        }
    }
}
