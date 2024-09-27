using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField]
    private KitchenObjectsSO kitchenObjectSO;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public event EventHandler OnPlayerGrabbedObject;

    public override void Interact(Player player)
    {
        // spawn kitchen object
        if (!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }
}
