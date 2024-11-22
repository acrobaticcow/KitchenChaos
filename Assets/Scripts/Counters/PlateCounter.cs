using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateAdded;
    public event EventHandler OnPlateRemoved;

    private enum ChangePlateAmountMode
    {
        Add,
        Take
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
        if (!IsServer)
        {
            return;
        }

        if (plateAmount < plateAmountMax)
        {
            platecounterTimer += Time.deltaTime;

            if (platecounterTimer > platecounterTimerMax)
            {
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateCLientRpc();
    }

    [ClientRpc]
    private void SpawnPlateCLientRpc()
    {
        platecounterTimer = 0;

        ChangePlateAmount(ChangePlateAmountMode.Add);
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            // KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
            // ChangePlateAmount(ChangePlateAmountMode.Take);
            KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        ChangePlateAmount(ChangePlateAmountMode.Take);
    }

    private void ChangePlateAmount(ChangePlateAmountMode mode)
    {
        if (mode == ChangePlateAmountMode.Add)
        {
            OnPlateAdded?.Invoke(this, EventArgs.Empty);
            plateAmount++;
        }
        else
        {
            OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            plateAmount--;
        }
    }
}
