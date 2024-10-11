using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField]
    private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList;

    private float waitingRecipeTimer;
    private readonly float waitingRecipeTimerMax = 4f;
    private readonly int maxWaitingRecipe = 4;
    private int deliveredRecipeCount;

    private void Awake()
    {
        waitingRecipeSOList = new();
        Instance = this;
    }

    private void Update()
    {
        waitingRecipeTimer -= Time.deltaTime;
        if (waitingRecipeTimer <= 0f)
        {
            waitingRecipeTimer = waitingRecipeTimerMax;

            if (waitingRecipeSOList.Count < maxWaitingRecipe)
            {
                var randomRecipeSO = recipeListSO.recipeSOList[
                    UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)
                ];
                waitingRecipeSOList.Add(randomRecipeSO);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliveredRecipe(PlateKitchenObject plateKitchenObject)
    {
        foreach (var waitingRecipeSO in waitingRecipeSOList)
            if (
                plateKitchenObject
                    .GetKitchenObjectList()
                    .TrueForAll(kitchenObjectSO =>
                        waitingRecipeSO.recipeSOList.Contains(kitchenObjectSO)
                    )
            )
            {
                Debug.Log(waitingRecipeSO.recipeName + " Delivered !");
                waitingRecipeSOList.Remove(waitingRecipeSO);
                deliveredRecipeCount++;
                OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                break;
            }
    }

    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetDeliveredRecipeCount()
    {
        return deliveredRecipeCount;
    }
}
