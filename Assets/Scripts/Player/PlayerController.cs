using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FloatCapsule))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Input))]
public class PlayerController : MonoBehaviour
{
    private WreckDiverInputActions diverInputActions;
    private Movement movement;
    private PlayerCamera camera;
    private FloatCapsule floatCapsule;
    private Interactor interactor;
    private LookPoint lookPoint;
    private ItemManager itemManager;
    private Helmet helmet;
    private bool isBoosting;

    private void Awake()
    {
        floatCapsule = GetComponent<FloatCapsule>();
        camera = GetComponent<PlayerCamera>();
        movement = GetComponent<Movement>();
        diverInputActions = new WreckDiverInputActions();
        itemManager = GetComponent<ItemManager>();
        interactor = transform.GetChild(0).GetComponentInChildren<Interactor>(); //This is a hacky way to get the Interactor component, it should be done in a better way
        lookPoint = transform.GetChild(0).GetComponentInChildren<LookPoint>(); //This is a hacky way to get the LookPoint component, it should be done in a better way
        helmet = GetComponentInChildren<Helmet>();
        if (interactor == null)
        {
            Debug.LogError("Interactor not found, make sure camera is the first child of the player, and that the Interactor is a child of the camera");
        }
        if (lookPoint == null)
        {
            Debug.LogError("LookPoint not found, make sure camera is the first child of the player, and that the LookPoint is a child of the camera");
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        //Sets all rotations to 0
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
    }

    private void OnEnable()
    {
        DeviceListener.SetupEvents();
        var movement = diverInputActions.Movement;
        movement.Move.performed += OnMove;
        movement.Look.performed += OnLook;
        movement.Jump.performed += OnJump;
        movement.Jump.started += OnJump;
        movement.Jump.canceled += OnJump;
        movement.Boost.performed += OnBoost;
        movement.Enable();
        var interact = diverInputActions.Interact;
        interact.Select.started += OnSelect;
        interact.Enable();
        var player = diverInputActions.Player;
        player.Pause.performed += OnPause;
        player.Enable();
        var inventory = diverInputActions.Inventory;
        inventory.PrimaryUse.started += OnPrimaryUse;
        inventory.PrimaryUse.performed += OnPrimaryUse;
        inventory.PrimaryUse.canceled += OnPrimaryUse;
        inventory.SecondaryUse.started += OnSecondaryUse;
        inventory.SecondaryUse.performed += OnSecondaryUse;
        inventory.SecondaryUse.canceled += OnSecondaryUse;
        inventory.TertiaryUse.started += OnTertiaryUse;
        inventory.TertiaryUse.performed += OnTertiaryUse;
        inventory.TertiaryUse.canceled += OnTertiaryUse;
        inventory.CycleUp.started += OnCycleUp;
        inventory.CycleDown.started += OnCycleDown;
        inventory.Enable();
    }

    private void OnCycleDown(InputAction.CallbackContext context)
    {
        itemManager.ChangeItem(-1);
    }

    private void OnCycleUp(InputAction.CallbackContext context)
    {
        itemManager.ChangeItem(1);
    }

    private void OnPrimaryUse(InputAction.CallbackContext context)
    {
        itemManager.PrimaryUse(context);
    }

    private void OnSecondaryUse(InputAction.CallbackContext context)
    {
        itemManager.SecondaryUse(context);
    }

    private void OnTertiaryUse(InputAction.CallbackContext context)
    {
        itemManager.TertiaryUse(context);
    }

    private void OnDisable()
    {
        var movement = diverInputActions.Movement;
        movement.Move.performed -= OnMove;
        movement.Look.performed -= OnLook;
        movement.Jump.performed -= OnJump;
        movement.Jump.started -= OnJump;
        movement.Jump.canceled -= OnJump;
        movement.Boost.performed -= OnBoost;
        movement.Disable();
        var interact = diverInputActions.Interact;
        interact.Select.started -= OnSelect;
        interact.Deselect.started -= OnDeselect;
        interact.Disable();
        var player = diverInputActions.Player;
        player.Pause.performed -= OnPause;
        player.Disable();
        var inventory = diverInputActions.Inventory;
        inventory.PrimaryUse.started -= OnPrimaryUse;
        inventory.PrimaryUse.performed -= OnPrimaryUse;
        inventory.PrimaryUse.canceled -= OnPrimaryUse;
        inventory.SecondaryUse.started -= OnSecondaryUse;
        inventory.SecondaryUse.performed -= OnSecondaryUse;
        inventory.SecondaryUse.canceled -= OnSecondaryUse;
        inventory.TertiaryUse.started -= OnTertiaryUse;
        inventory.TertiaryUse.performed -= OnTertiaryUse;
        inventory.TertiaryUse.canceled -= OnTertiaryUse;
        inventory.CycleUp.started -= OnCycleUp;
        inventory.CycleDown.started -= OnCycleDown;
        inventory.Disable();
    }

    // void OnChangeItem(InputValue value)
    // {
    //     var val = (int)value.Get<float>();
    //     print(val);
    //     itemManager.ChangeItem(val);
    // }
    //
    // void OnItemPrimary(InputValue value)
    // {
    //     var val = value.isPressed;
    //
    //     itemManager.PrimaryUse(val);
    // }
    //
    // void OnItemSecondary(InputValue value)
    // {
    //     var val = value.isPressed;
    //     itemManager.SecondaryUse(val);
    // }
    //
    // void OnItemTertiary(InputValue value)
    // {
    //     var val = value.isPressed;
    //     itemManager.TertiaryUse(val);
    // }

    public void FixedUpdate()
    {
        if (!movement.enabled) return;
        movement.Move(diverInputActions.Movement.Move.ReadValue<Vector2>(), isBoosting);
        if (DeviceListener.CurrentDevice is not DeviceType.Gamepad) return;
        camera.Look(diverInputActions.Movement.Look.ReadValue<Vector2>());
        lookPoint.UpdatePosition(diverInputActions.Movement.Look.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        helmet.Movement = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (DeviceListener.CurrentDevice is DeviceType.Gamepad) return;
        camera.Look(context.ReadValue<Vector2>());
        lookPoint.UpdatePosition(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        floatCapsule.Jump(context.started, !context.started);
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        isBoosting = context.ReadValueAsButton();
    }

    private double lastKeyPress = 0;

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!interactor.Select()) return;
        diverInputActions.Interact.Select.started -= OnSelect;
        StartCoroutine(DelayedSelect());
    }

    IEnumerator DelayedSelect()
    {
        yield return new WaitForSeconds(0.5f);
        var interact = diverInputActions.Interact;
        interact.Deselect.started += OnDeselect;
    }

    IEnumerator DelayedDeselect()
    {
        yield return new WaitForSeconds(0.5f);
        var interact = diverInputActions.Interact;
        interact.Select.started += OnSelect;
    }

    private void OnDeselect(InputAction.CallbackContext context)
    {
        interactor.Deselect();
        diverInputActions.Interact.Deselect.started -= OnDeselect;
        StartCoroutine(DelayedDeselect());
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.canceled || context.started || !movement.enabled) return;
        if (diverInputActions.Movement.enabled) diverInputActions.Movement.Disable();
        else diverInputActions.Movement.Enable();
    }

    public void DisableMovement()
    {
        diverInputActions.Movement.Disable();
        diverInputActions.Inventory.Disable();
        movement.StopMoving();
        movement.enabled = false;
    }

    public void EnableMovement()
    {
        diverInputActions.Movement.Enable();
        diverInputActions.Inventory.Enable();
        movement.StartMoving();
        movement.enabled = true;
    }
}
