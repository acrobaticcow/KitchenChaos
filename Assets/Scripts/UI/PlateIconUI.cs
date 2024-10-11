using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField]
    private PlateKitchenObject plateKitchenObject;

    [SerializeField]
    private IconTemplateUI iconTemplateUI;

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(
        object sender,
        PlateKitchenObject.OnIngredientsAddedEventArgs e
    )
    {
        var iconTemplateUIInstance = Instantiate(iconTemplateUI, transform);
        iconTemplateUIInstance.SetIcon(e.kitchenObjectSO.Sprite);
        iconTemplateUIInstance.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        plateKitchenObject.OnIngredientAdded -= PlateKitchenObject_OnIngredientAdded;
    }
}
