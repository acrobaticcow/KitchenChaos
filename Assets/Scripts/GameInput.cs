using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;
    public event EventHandler OnInteraction;
    public event EventHandler OnInteractionAlternate;
    public event EventHandler OnPause;
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
        playerInputAction.Player.Pause.performed += PlayerPause;
        Instance = this;
    }

    public void OnDestroy()
    {
        playerInputAction.Player.Interact.performed -= PlayerInteract;
        playerInputAction.Player.InteractAlternate.performed -= PlayerInteractAlternate;
        playerInputAction.Player.Pause.performed -= PlayerPause;
        playerInputAction.Dispose();
    }

    private void PlayerPause(InputAction.CallbackContext context)
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerInteractAlternate(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGamePlaying())
            OnInteractionAlternate?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerInteract(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsGamePlaying())
            OnInteraction?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        _inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();
    }
}
