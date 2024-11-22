using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    [SerializeField]
    private CuttingRecipeSO[] cuttingRecipeSOs;
    private int cuttingProgress;

    public event EventHandler OnCut;
    public event EventHandler<IHasProgress.ProgressChangedEventArgs> OnProgressChanged;

    public override void Interact(Player player)
    {
        // if the counter doesn't have kitchen object
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    var kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);
                    InteractLogicPlateCoutnerServerRpc();
                }
            }
        }
        else // if the counter have kitchen object
        {
            if (!player.HasKitchenObject()) // if the player doesn't have kitchen object
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
            else if (
                player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)
            ) // if the player is holding a plate kitchen object
            {
                if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO()))
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlateCoutnerServerRpc()
    {
        InteractLogicPlateCoutnerClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlateCoutnerClientRpc()
    {
        cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.ProgressChangedEventArgs(0f));
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }

    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;

        var cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

        OnProgressChanged?.Invoke(
            this,
            new IHasProgress.ProgressChangedEventArgs(
                (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            )
        );

        OnCut?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        var cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {
            var outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }

    private bool HasRecipeForInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        var cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return true;
        }
        return false;
    }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        var cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        return null;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectsSO input)
    {
        foreach (var recipe in cuttingRecipeSOs)
        {
            if (recipe.input == input)
            {
                return recipe;
            }
        }
        return null;
    }
}
