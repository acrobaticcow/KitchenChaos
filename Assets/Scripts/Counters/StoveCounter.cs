using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    private NetworkVariable<float> fryingTimer = new(0f);
    private NetworkVariable<float> burningTimer = new(0f);

    private NetworkVariable<State> state = new(State.Idle);

    [SerializeField]
    private FryingRecipeSO[] fryingRecipeSOs;
    private FryingRecipeSO fryingRecipeSO;
    private FryingRecipeSO friedRecipeSO;
    public event EventHandler<IHasProgress.ProgressChangedEventArgs> OnProgressChanged;

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = newValue });
        if (newValue == State.Burned || newValue == State.Idle)
            OnProgressChanged?.Invoke(this, new IHasProgress.ProgressChangedEventArgs(0f));
    }

    private void BurningTimer_OnValueChanged(float prev, float current)
    {
        float burningTimerMax = friedRecipeSO?.fryingTimerMax ?? 1f;
        OnProgressChanged?.Invoke(
            this,
            new IHasProgress.ProgressChangedEventArgs(current / burningTimerMax)
        );
    }

    private void FryingTimer_OnValueChanged(float prev, float current)
    {
        float fryingTimerMax = fryingRecipeSO?.fryingTimerMax ?? 1f;
        OnProgressChanged?.Invoke(
            this,
            new IHasProgress.ProgressChangedEventArgs(current / fryingTimerMax)
        );
    }

    private void Update()
    {
        if (!IsServer)
            return;

        switch (state.Value)
        {
            case State.Idle:
                friedRecipeSO = null;
                fryingRecipeSO = null;
                break;
            case State.Frying:
                fryingTimer.Value += Time.deltaTime;
                if (fryingRecipeSO == null)
                    Debug.LogError("fryingRecipeSO should be available in State.Frying !");

                if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                    state.Value = State.Fried;
                    burningTimer.Value = 0f;
                }
                break;
            case State.Fried:
                burningTimer.Value += Time.deltaTime;
                if (friedRecipeSO == null)
                {
                    if (!HasRecipeForInput(fryingRecipeSO.output)) // ! performant implication
                        return;

                    SetBurningRecipeSOClientRpc(
                        KitchenGameObjectMultiplayer.Instance.GetKitchenObjectSoIndex(
                            fryingRecipeSO.output
                        )
                    );
                }

                if (burningTimer.Value > friedRecipeSO.fryingTimerMax)
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    KitchenObject.SpawnKitchenObject(friedRecipeSO.output, this);

                    state.Value = State.Burned;
                }
                break;
            case State.Burned:
                fryingTimer.Value = 0f;
                burningTimer.Value = 0f;
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
                KitchenObject kitchenObject = player.GetKitchenObject();
                kitchenObject.SetKitchenObjectParent(this);

                InteractLogicPlaceObjectOnCounterServerRpc(
                    KitchenGameObjectMultiplayer.Instance.GetKitchenObjectSoIndex(
                        kitchenObject.GetKitchenObjectSO()
                    )
                );
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                SetStateIdleServerRpc();
            }
            else if (
                player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)
            ) // if the player is holding a plate kitchen object
            {
                if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    SetStateIdleServerRpc();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0f;
        state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        var kitchenObjectSO = KitchenGameObjectMultiplayer.Instance.GetKitchenObjectSOFromIndex(
            kitchenObjectSOIndex
        );
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        var kitchenObjectSO = KitchenGameObjectMultiplayer.Instance.GetKitchenObjectSOFromIndex(
            kitchenObjectSOIndex
        );
        friedRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
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
