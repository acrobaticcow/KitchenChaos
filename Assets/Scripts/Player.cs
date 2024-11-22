using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyPlayerSpawned;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }

    [SerializeField]
    private float speed = 7f;

    [SerializeField]
    private float turnSpeed = 10f;

    [SerializeField]
    private LayerMask counterLayerMask;

    [SerializeField]
    private LayerMask collisionLayerMask;

    [SerializeField]
    private List<Vector3> spawnPositionList;
    public bool IsWalking { get; private set; }

    private Vector3 lastInteractiveDir;
    private BaseCounter selectedCounter;
    public static Player LocalInstance { get; private set; }
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    [SerializeField]
    private Transform kitchenObjectHoldPoint;

    private KitchenObject kitchenObject;

    private void Start()
    {
        GameInput.Instance.OnInteraction += GameInput_OnInteractionPerformed;
        GameInput.Instance.OnInteractionAlternate += GameInput_OnInteractionAlternatePerformed;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
            LocalInstance = this;
        transform.position = spawnPositionList[(int)OwnerClientId];
        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        HandleMovement();
        // HandleMovementServerAuth();
        HandleInteraction();
    }

    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    private void GameInput_OnInteractionPerformed(object sender, EventArgs e)
    {
        if (selectedCounter != null)
            selectedCounter.Interact(this);
    }

    private void GameInput_OnInteractionAlternatePerformed(object sender, EventArgs e)
    {
        if (selectedCounter != null)
            selectedCounter.InteractAlternate(this);
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = GameInput.Instance.InputVectorNormalized;
        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
        float interactiveDistance = 2f;

        if (moveDir != Vector3.zero)
        {
            lastInteractiveDir = moveDir;
        }

        bool isCounterInteracted = Physics.Raycast(
            transform.position,
            lastInteractiveDir,
            out RaycastHit rayCastHit,
            interactiveDistance,
            counterLayerMask
        );

        if (isCounterInteracted)
        {
            if (rayCastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (selectedCounter != baseCounter)
                    SetSelectedCounter(baseCounter);
            }
            else
                SetSelectedCounter(null);
        }
        else
            SetSelectedCounter(null);
    }

    private void SetSelectedCounter(BaseCounter baseCounter)
    {
        selectedCounter = baseCounter;
        OnSelectedCounterChanged?.Invoke(
            this,
            new OnSelectedCounterChangedEventArgs { selectedCounter = selectedCounter }
        );
    }

    private void HandleMovementServerAuth()
    {
        Vector2 inputVector = GameInput.Instance.InputVectorNormalized;
        HandleMovementServerRpc(inputVector);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandleMovementServerRpc(Vector2 inputVector)
    {
        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = speed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * playerHeight,
            playerRadius,
            moveDir,
            moveDistance,
            collisionLayerMask
        );
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove =
                moveDir.x != 0
                && !Physics.CapsuleCast(
                    transform.position,
                    transform.position + Vector3.up * playerHeight,
                    playerRadius,
                    moveDirX,
                    moveDistance,
                    collisionLayerMask
                );
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove =
                    moveDir.z != 0
                    && !Physics.CapsuleCast(
                        transform.position,
                        transform.position + Vector3.up * playerHeight,
                        playerRadius,
                        moveDirZ,
                        moveDistance,
                        collisionLayerMask
                    );

                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDistance * moveDir;
        }
        IsWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.InputVectorNormalized;
        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
        float playerRadius = .7f;
        float moveDistance = speed * Time.deltaTime;
        bool canMove = !Physics.BoxCast(
            transform.position,
            Vector3.one * playerRadius,
            moveDir,
            Quaternion.identity,
            moveDistance,
            collisionLayerMask
        );
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove =
                moveDir.x != 0
                && !Physics.BoxCast(
                    transform.position,
                    Vector3.one * playerRadius,
                    moveDirX,
                    Quaternion.identity,
                    moveDistance,
                    collisionLayerMask
                );
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove =
                    moveDir.z != 0
                    && !Physics.BoxCast(
                        transform.position,
                        Vector3.one * playerRadius,
                        moveDirZ,
                        Quaternion.identity,
                        moveDistance,
                        collisionLayerMask
                    );

                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDistance * moveDir;
        }
        IsWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * turnSpeed);
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public NetworkObject GetNetWorkObject()
    {
        return NetworkObject;
    }
}
