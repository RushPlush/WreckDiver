using AmplifyShaderEditor;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FloatCapsule))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Input))]
public class PlayerController : MonoBehaviour
{
    private Input input;
    private Movement movement;
    private PlayerCamera camera;
    private FloatCapsule floatCapsule;
    private Interactor interactor;
    private LookPoint lookPoint;
    private ItemManager itemManager;

    private void Awake()
    {
        floatCapsule = GetComponent<FloatCapsule>();
        camera = GetComponent<PlayerCamera>();
        movement = GetComponent<Movement>();
        input = GetComponent<Input>();
        itemManager = GetComponent<ItemManager>();
        interactor = transform.GetChild(0).GetComponentInChildren<Interactor>(); //This is a hacky way to get the Interactor component, it should be done in a better way
        lookPoint = transform.GetChild(0).GetComponentInChildren<LookPoint>(); //This is a hacky way to get the LookPoint component, it should be done in a better way
        if(interactor == null)
        {
            Debug.LogError("Interactor not found, make sure camera is the first child of the player, and that the Interactor is a child of the camera");
        }
        if(lookPoint == null)
        {
            Debug.LogError("LookPoint not found, make sure camera is the first child of the player, and that the LookPoint is a child of the camera");
        }
    }
    private void Start()
    {
        //Sets all rotations to 0
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
    }
    private void Update()
    {
        camera.Look(input.lookVector);
        lookPoint.UpdatePosition(input.lookVector, input.usingKeyboard);
        floatCapsule.Jump(input.jumpTrigger, input.isJumping);
        if(input.interactTrigger)
        {
            interactor.Interact();
        }
        //m_ItemManager.ChangeItem(m_Input.itemChange);
        //if(m_Input.isUsingPrimary)
        //{
        //    m_ItemManager.PrimaryUse();
        //}
        //if(m_Input.isUsingSecondary)
        //{
        //    m_ItemManager.SecondaryUse();
        //}
        //if(m_Input.isUsingTertiary)
        //{
        //    m_ItemManager.TertiaryUse();
        //}
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
    private void FixedUpdate() {    
        movement.Move(input.moveDirection, input.isBoosting);
    }
}
