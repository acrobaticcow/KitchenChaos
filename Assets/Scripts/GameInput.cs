using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteraction;
    public event EventHandler OnInteractionAlternate;
    private Vector2 _inputVector = new(0, 0);
    public Vector2 InputVectorNormalized
    {
        get => _inputVector.normalized;
        private set => _inputVector = value;
    }
    private PlayerInputAction playerInputAction;

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
        playerInputAction.Enable();
        playerInputAction.Player.Interact.performed += PlayerInteract;
        playerInputAction.Player.InteractAlternate.performed += PlayerInteractAlternate;
    }

    private void PlayerInteractAlternate(InputAction.CallbackContext context)
    {
        OnInteractionAlternate?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerInteract(InputAction.CallbackContext context)
    {
        OnInteraction?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        _inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
    }
}
