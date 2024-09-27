using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler<OnPlateAddedEventArgs> OnPlateAmountChanged;

    public class OnPlateAddedEventArgs : EventArgs
    {
        public int amount;
    }

    [SerializeField]
    private float platecounterTimerMax = 4f;

    [SerializeField]
    private KitchenObjectsSO plateKitchenObjectSO;

    [SerializeField]
    private int plateAmountMax = 4;
    private float platecounterTimer;

    private int plateAmount = 0;

    private void Update()
    {
        platecounterTimer += Time.deltaTime;

        if (platecounterTimer > platecounterTimerMax)
        {
            platecounterTimer = 0;

            if (plateAmount < plateAmountMax)
            {
                ChangePlateAmount(plateAmount + 1);
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
            ChangePlateAmount(plateAmount - 1);
        }
        else if (plateAmount < plateAmountMax)
        {
            Destroy(player.GetKitchenObject().gameObject);
            ChangePlateAmount(plateAmount + 1);
        }
    }

    private void ChangePlateAmount(int amount)
    {
        plateAmount = amount;
        OnPlateAmountChanged?.Invoke(this, new OnPlateAddedEventArgs { amount = plateAmount });
    }
}
