using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction; 
    //To enable the new Input System. 
    PlayerInputActions playerInputActions;
    private void Awake()
    {

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        //Interaction
        playerInputActions.Player.Interact.performed += Interact_performed;
    
    }


    public Vector2 GetMovementVectorNormalized() {

        //Use explicit coding practice, thats why it says private 
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

       
        //Normalize to keep same speed of diagonals and strafes
        inputVector = inputVector.normalized;

        return inputVector;
    }
    //Auto Function after doing += on Interaction press TAB
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }
}

 
