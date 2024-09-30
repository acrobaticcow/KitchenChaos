using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    [SerializeField]
    private CuttingRecipeSO[] cuttingRecipeSOs;
    private int cuttingProgress;

    public event EventHandler OnCut;
    public event EventHandler<IHasProgress.ProgressChangedEventArgs> ProgressChanged;

    public override void Interact(Player player)
    {
        // if the counter doesn't have kitchen object
        if (!HasKitchenObject())
        {
            if (
                player.HasKitchenObject()
                && HasRecipeForInput(player.GetKitchenObject().GetKitchenObjectSO())
            )
            {
                var cuttingRecipeSO = GetCuttingRecipeSOWithInput(
                    player.GetKitchenObject().GetKitchenObjectSO()
                );

                player.GetKitchenObject().SetKitchenObjectParent(this);

                cuttingProgress = 0;

                ProgressChanged?.Invoke(
                    this,
                    new IHasProgress.ProgressChangedEventArgs(
                        (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                    )
                );
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
                    GetKitchenObject().DestroySelf();
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;

            var cuttingRecipeSO = GetCuttingRecipeSOWithInput(
                GetKitchenObject().GetKitchenObjectSO()
            );

            ProgressChanged?.Invoke(
                this,
                new IHasProgress.ProgressChangedEventArgs(
                    (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                )
            );

            OnCut?.Invoke(this, EventArgs.Empty);

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                var outputKitchenObjectSO = GetOutputForInput(
                    GetKitchenObject().GetKitchenObjectSO()
                );
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
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
