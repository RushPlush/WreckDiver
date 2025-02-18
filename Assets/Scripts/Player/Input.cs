using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input : MonoBehaviour
{
    public Vector2 moveDirection { get; private set; }
    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
        Console.WriteLine(moveDirection + " = move direction" );
    }

    public Vector2 lookVector { get; private set; }
    public void OnLook(InputValue value) => lookVector = value.Get<Vector2>();
    public bool isJumping { get; private set; }
    private bool previousJumpValue = false;
    public bool jumpTrigger { get; private set; }
    public void OnJump(InputValue value)
    {
        isJumping = value.isPressed;
        if (!previousJumpValue) // to make sure it's an impulse. single frame. 
        {
            jumpTrigger = isJumping;
        }
        previousJumpValue = isJumping;
    }
    public bool isCrouching { get; private set; }
    public void OnCrouch(InputValue value) => isCrouching = value.isPressed;
    public bool isBoosting { get; private set; }
    public void OnBoost(InputValue value) => isBoosting = value.isPressed;
    public bool isPausing { get; private set; }
    public void OnPause(InputValue value) => isPausing = value.isPressed; // might have to do a state check here as well, but it is set as initial state check. 
    public bool isOpeningInventory { get; private set; }
    public void OnOpenInventory(InputValue value) => isOpeningInventory = value.isPressed;
    public bool isInteracting { get; private set; }
    public bool interactTrigger { get; private set; }
    private bool previousInteractValue = false;
    public void OnInteract(InputValue value)
    {
        isInteracting = value.isPressed;
        if(!previousInteractValue)
            interactTrigger = isInteracting;
        previousInteractValue = isInteracting;
    }
    public bool usingKeyboard { get; private set; }
    
    public void OnAnyKeyboard(InputValue value)
    {
        usingKeyboard = true;
        print("Keyboard input detected");
    }
    public void OnAnyController(InputValue value)
    {
        usingKeyboard = false;
        print("Gamepad input detected");
    }

    //for on pause, and on open inventory, we might want to swap over to a different action map, off of player and into either a dedicated inventory/pause action map, or into a more general UI navigation action map. 
    //this would also enable us to have menus without pausing the game.
    private void LateUpdate()
    {
        //previousJumpValue = isJumping;
        //previousInteractValue = isInteracting;
        //isOpeningInventory = false;
        jumpTrigger = false;
        interactTrigger = false;
        //isBoosting  = false;
        //isPausing   = false;
        //isCrouching = false;
    }
}
