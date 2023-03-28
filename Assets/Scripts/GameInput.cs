using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;
    
    private PlayerInputActions playerInputActions;
    public static GameInput Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        
        playerInputActions.Player.Interact.performed += InteractOnperformed; 
        playerInputActions.Player.InteractAlternate.performed += InteractAlternateOnperformed;
        playerInputActions.Player.Pause.performed += PauseOnperformed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= InteractOnperformed; 
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternateOnperformed;
        playerInputActions.Player.Pause.performed -= PauseOnperformed;
        
        // playerInputActions is a bit sticky so we explicitly kill it. When a player restarts the game then a new one will be created
        playerInputActions.Dispose();
    }

    private void PauseOnperformed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractOnperformed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
    private void InteractAlternateOnperformed(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;

        return inputVector;
    }
}
