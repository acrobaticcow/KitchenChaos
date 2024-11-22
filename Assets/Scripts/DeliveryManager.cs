using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField]
    private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList;

    private float waitingRecipeTimer = 4f;
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
        if (!IsServer)
            return;

        waitingRecipeTimer -= Time.deltaTime;
        if (waitingRecipeTimer <= 0f)
        {
            waitingRecipeTimer = waitingRecipeTimerMax;

            if (waitingRecipeSOList.Count < maxWaitingRecipe)
            {
                SpawnNewWaitingRecipeClientRpc(
                    UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)
                );
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        var waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliveredRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            var waitingRecipeSO = waitingRecipeSOList[i];
            if (
                plateKitchenObject
                    .GetKitchenObjectList()
                    .TrueForAll(kitchenObjectSO =>
                        waitingRecipeSO.recipeSOList.Contains(kitchenObjectSO)
                    )
            )
            {
                DeliverCorrectRecipeServerRpc(i);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOIndex)
    {
        var waitingRecipeSO = waitingRecipeSOList[waitingRecipeSOIndex];
        Debug.Log(waitingRecipeSO.recipeName + " Delivered !");
        waitingRecipeSOList.RemoveAt(waitingRecipeSOIndex);
        deliveredRecipeCount++;
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
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
