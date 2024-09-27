using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField]
    private float speed = 7f;

    [SerializeField]
    private float turnSpeed = 10f;

    [SerializeField]
    private LayerMask counterMask;
    public bool IsWalking { get; private set; }

    [SerializeField]
    private GameInput gameInput;
    private Vector3 lastInteractiveDir;
    private BaseCounter selectedCounter;
    public static Player Instance { get; private set; }
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    [SerializeField]
    private Transform kitchenObjectHoldPoint;

    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("There are more than one instance of Player !!!");
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteraction += GameInput_OnInteractionPerformed;
        gameInput.OnInteractionAlternate += GameInput_OnInteractionAlternatePerformed;
    }

    private void Update()
    {
        HandleMovement();
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
        Vector2 inputVector = gameInput.InputVectorNormalized;
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
            counterMask
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

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.InputVectorNormalized;
        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);
        float playerRadius = .7f;
        float playerHeight = 2f;
        float moveDistance = speed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * playerHeight,
            playerRadius,
            moveDir,
            moveDistance
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
                    moveDistance
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
                        moveDistance
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
}
