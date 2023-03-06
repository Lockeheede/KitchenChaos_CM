using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Start of Singleton Pattern
    public static Player Instance { get; private set; }

    public event EventHandler <OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public ClearCounter selectedCounter;
    }
    //A bool that interacts with the PlayerAnimator script
    private bool isWalking;
    //Never make fields public, instead make SerializeField
    private Vector3 lastInteractDirection;
    //For Selected Visuals Logic
    private ClearCounter selectedCounter;
    [SerializeField] private float moveSpeed = 10f;
    //Refactor to the new input system. See notes and 'GameInput.cs' in game
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    //Change the countersLayerMask in the engine to Counters

    //Starting from scratch, erase all the logic first

    private void Awake() {
        if (Instance != null)
        {
            Debug.Log("There is more than one player Instance!");
        }
        Instance = this;
    }
    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;  
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact();
        }
    }

    private void Update()
   {
        HandleMovement(); 
        HandleInteractions();
   }
   public bool IsWalking() 
   {
        return isWalking;
   }

   private void HandleMovement() 
   {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3 (inputVector.x, 0, inputVector.y);

        //Raycast to test if anything is in the path
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirection, moveDistance);

    if (!canMove)
    {
        //Cannot move towards direction

        //Only on the X
        Vector3 moveDirectionX = new Vector3 (moveDirection.x, 0, 0).normalized;
        canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionX, moveDistance);

        if (canMove)
        {
            moveDirection = moveDirectionX;
        } else {
            //Cannot move only on the X

            //Only Z movement
            Vector3 moveDirectionZ = new Vector3 (0, 0, moveDirection.z).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionZ, moveDistance);

            if (canMove)
            {
                moveDirection = moveDirectionZ;
            } //else cannot move in any direction
        }
    }

    //Use DeltaTime to be frame rate independent 
    if (canMove)
    {
        transform.position += moveDirection * moveDistance;
    }

    isWalking = moveDirection != Vector3.zero;
    float rotateSpeed = 10f;
    transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
   }

   private void HandleInteractions()
   {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3 (inputVector.x, 0, inputVector.y);

        if(moveDirection != Vector3.zero)
        {
            lastInteractDirection = moveDirection;
        }

        float interactDistance = 2f;

        //Using raycast again to check if anything is in the path. 
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        //Out parameter is for outputting information, notice the out keyword
        {
            if(raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                //Like Get Component except it automatically does the fails
                //Has ClearCounter Component
                if(clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
                else 
                {
                    SetSelectedCounter(null);
                }
            }

        } 
            else
            {
                SetSelectedCounter(null);
            }
    }

    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs { 
            selectedCounter = selectedCounter
        });
    }

}   
