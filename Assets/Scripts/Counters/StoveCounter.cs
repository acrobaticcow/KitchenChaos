using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    private State state;

    [SerializeField]
    private FryingRecipeSO[] fryingRecipeSOs;
    private FryingRecipeSO fryingRecipeSO;
    private FryingRecipeSO friedRecipeSO;
    public event EventHandler OnFry;
    public event EventHandler<IHasProgress.ProgressChangedEventArgs> ProgressChanged;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                friedRecipeSO = null;
                fryingRecipeSO = null;
                break;
            case State.Frying:
                if (fryingRecipeSO == null)
                    Debug.LogError("fryingRecipeSO should be available in State.Frying !");

                // fryingTimer += Time.deltaTime;

                GetKitchenObject().CurrentActionProgress += Time.deltaTime;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });

                if (GetKitchenObject().CurrentActionProgress <= fryingRecipeSO.fryingTimerMax)
                    ProgressChanged?.Invoke(
                        this,
                        new IHasProgress.ProgressChangedEventArgs(
                            GetKitchenObject().CurrentActionProgress / fryingRecipeSO.fryingTimerMax
                        )
                    );
                else
                {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                    state = State.Fried;
                }
                break;
            case State.Fried:
                if (friedRecipeSO == null)
                {
                    if (!HasRecipeForInput(fryingRecipeSO.output)) // ! performant implication
                        return;
                    friedRecipeSO = GetFryingRecipeSOWithInput(fryingRecipeSO.output);
                }

                GetKitchenObject().CurrentActionProgress += Time.deltaTime;

                if (GetKitchenObject().CurrentActionProgress <= friedRecipeSO.fryingTimerMax)
                    ProgressChanged?.Invoke(
                        this,
                        new IHasProgress.ProgressChangedEventArgs(
                            GetKitchenObject().CurrentActionProgress / friedRecipeSO.fryingTimerMax
                        )
                    );
                else
                {
                    GetKitchenObject().DestroySelf();
                    KitchenObject.SpawnKitchenObject(friedRecipeSO.output, this);

                    state = State.Burned;
                }
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
                break;
            case State.Burned:
                ProgressChanged?.Invoke(this, new IHasProgress.ProgressChangedEventArgs(0f));
                break;
        }
    }

    public class OnProgressChanged_EventArgs : EventArgs
    {
        public float progressNormalized;
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // has kitchen object and satisfy recipe
            if (
                player.HasKitchenObject()
                && HasRecipeForInput(player.GetKitchenObject().GetKitchenObjectSO())
            )
            {
                fryingRecipeSO = GetFryingRecipeSOWithInput(
                    player.GetKitchenObject().GetKitchenObjectSO()
                );

                player.GetKitchenObject().SetKitchenObjectParent(this);

                state = State.Frying;

                OnFry?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
            }
            else if (
                player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)
            ) // if the player is holding a plate kitchen object
            {
                if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO()))
                {
                    GetKitchenObject().DestroySelf();
                    state = State.Idle;
                }
            }
            ProgressChanged?.Invoke(this, new IHasProgress.ProgressChangedEventArgs(0f));
        }
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = state });
    }

    public override void InteractAlternate(Player player) { }

    private bool HasRecipeForInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        var fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return true;
        }
        return false;
    }

    private KitchenObjectsSO GetOutputForInput(KitchenObjectsSO inputKitchenObjectSO)
    {
        var cuttingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        return null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectsSO input)
    {
        foreach (var recipe in fryingRecipeSOs)
        {
            if (recipe.input == input)
            {
                return recipe;
            }
        }
        return null;
    }
}
