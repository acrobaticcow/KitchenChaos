using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField]
    private KitchenObjectsSO kitchenObjectSO;

    private void Update() { }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
            else if (
                player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)
            )
            {
                if (plateKitchenObject.TryAddIngredients(GetKitchenObject().GetKitchenObjectSO()))
                    GetKitchenObject().DestroySelf();
            }
            else if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                if (
                    plateKitchenObject.TryAddIngredients(
                        player.GetKitchenObject().GetKitchenObjectSO()
                    )
                )
                    player.GetKitchenObject().DestroySelf();
        }
    }
}
