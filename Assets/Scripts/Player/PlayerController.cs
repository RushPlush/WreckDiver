using System.Linq;
using AmplifyShaderEditor;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(FloatCapsule))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Input))]
public class PlayerController : MonoBehaviour, WreckDiverInputActions.IMovementActions, WreckDiverInputActions.IInteractActions, WreckDiverInputActions.IPlayerActions
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
        InputSystem.onDeviceChange += InputSystemOnDeviceChange;
        InputSystem.onEvent += InputSystemOnEvent;
        diverInputActions.Interact.AddCallbacks(this);
        diverInputActions.Interact.Enable();
        diverInputActions.Movement.AddCallbacks(this);
        diverInputActions.Movement.Enable();
        diverInputActions.Player.AddCallbacks(this);
        diverInputActions.Player.Enable();
    }

    InputDevice lastDevice = null;

    private void InputSystemOnEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (lastDevice == device) return;
        var eventType = eventPtr.type;
        if (eventType == StateEvent.Type)
            // PS4 controller likes to spam events so this filters them out
            if (!eventPtr.EnumerateChangedControls(device, 0.0001f).Any())
                return;

        lastDevice = device;
        camera.ChangeScheme(device is Gamepad);
    }

    private void InputSystemOnDeviceChange(InputDevice device, InputDeviceChange arg2)
    {
        if (lastDevice == device) return;
        lastDevice = device;
        camera.ChangeScheme(device is Gamepad);
    }

    private void OnDisable()
    {
        diverInputActions.Interact.Disable();
        diverInputActions.Movement.Disable();
        diverInputActions.Player.Disable();
    }

    void OnChangeItem(InputValue value)
    {
        var val = (int)value.Get<float>();
        print(val);
        itemManager.ChangeItem(val);
    }

    void OnItemPrimary(InputValue value)
    {
        var val = value.isPressed;

        itemManager.PrimaryUse(val);
    }

    void OnItemSecondary(InputValue value)
    {
        var val = value.isPressed;
        itemManager.SecondaryUse(val);
    }

    void OnItemTertiary(InputValue value)
    {
        var val = value.isPressed;
        itemManager.TertiaryUse(val);
    }

    public void FixedUpdate()
    {
        movement.Move(diverInputActions.Movement.Move.ReadValue<Vector2>(), isBoosting);
        if (lastDevice is not Gamepad) return;
        camera.Look(diverInputActions.Movement.Look.ReadValue<Vector2>());
        lookPoint.UpdatePosition(diverInputActions.Movement.Look.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        helmet.Movement = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (lastDevice is Gamepad) return;
        camera.Look(context.ReadValue<Vector2>());
        lookPoint.UpdatePosition(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        floatCapsule.Jump(context.started, !context.started);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        isBoosting = !context.canceled;
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            interactor.Interact();
        }
    }

    public void OnOpenInventory(InputAction.CallbackContext context)
    {
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.canceled || context.started) return;
        if(diverInputActions.Movement.enabled) diverInputActions.Movement.Disable();
        else diverInputActions.Movement.Enable();
    }
}
