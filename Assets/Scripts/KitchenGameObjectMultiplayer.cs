using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameObjectMultiplayer : NetworkBehaviour
{
    [SerializeField]
    private KitchenObjectsListSO kitchenObjectsListSO;
    public static KitchenGameObjectMultiplayer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnKitchenObject(
        KitchenObjectsSO kitchenObjectSO,
        IKitchenObjectParent kitchenObjectParent
    )
    {
        SpawnKitchenObjectServerRpc(
            GetKitchenObjectSoIndex(kitchenObjectSO),
            kitchenObjectParent.GetNetWorkObject()
        );
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(
        int kitchenObjectSOIndex,
        NetworkObjectReference kitchenObjectParentNOR
    )
    {
        KitchenObjectsSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.Prefab);
        NetworkObject kitchenObjectNetworkObject =
            kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        var kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObjectParentNOR.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        var kitchenObjectParent =
            kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSoIndex(KitchenObjectsSO kitchenObjectSO)
    {
        return kitchenObjectsListSO.kitchenObjectsListSO.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectsSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjectsListSO.kitchenObjectsListSO[kitchenObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNOR)
    {
        kitchenObjectNOR.TryGet(out NetworkObject kitchenObjectNetworkObject);
        var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObject);

        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNOR)
    {
        kitchenObjectNOR.TryGet(out NetworkObject kitchenObjectNetworkObject);
        var kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();
    }
}
