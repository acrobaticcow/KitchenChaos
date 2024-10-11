using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField]
    private Transform container;

    [SerializeField]
    private Transform recipeTemplateUI;

    private void Awake()
    {
        recipeTemplateUI.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManagerUI_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManagerUI_OnRecipeCompleted;

        UpdateVisual();
    }

    private void DeliveryManagerUI_OnRecipeCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManagerUI_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplateUI)
                continue;
            else
                Destroy(child.gameObject);
        }

        foreach (var waitingRecipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
        {
            var recipeTemplateUITransform = Instantiate(recipeTemplateUI, container);
            recipeTemplateUITransform.gameObject.SetActive(true);
            recipeTemplateUITransform
                .GetComponent<DeliveryManagerSingleUI>()
                .SetRecipeSO(waitingRecipeSO);
        }
    }
}
