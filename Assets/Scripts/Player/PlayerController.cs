using Player;
using UnityEngine;

[RequireComponent(typeof(FloatCapsule))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Input))]
public class PlayerController : MonoBehaviour
{
    private Input m_Input;
    private Movement m_Movement;
    private PlayerCamera m_Camera;
    private FloatCapsule m_FloatCapsule;
    private Interactor m_Interactor;
    private LookPoint m_LookPoint;

    private void Awake()
    {
        m_FloatCapsule = GetComponent<FloatCapsule>();
        m_Camera = GetComponent<PlayerCamera>();
        m_Movement = GetComponent<Movement>();
        m_Input = GetComponent<Input>();
        m_Interactor = transform.GetChild(0).GetComponentInChildren<Interactor>(); //This is a hacky way to get the Interactor component, it should be done in a better way
        m_LookPoint = transform.GetChild(0).GetComponentInChildren<LookPoint>(); //This is a hacky way to get the LookPoint component, it should be done in a better way
        if(m_Interactor == null)
        {
            Debug.LogError("Interactor not found, make sure camera is the first child of the player, and that the Interactor is a child of the camera");
        }
        if(m_LookPoint == null)
        {
            Debug.LogError("LookPoint not found, make sure camera is the first child of the player, and that the LookPoint is a child of the camera");
        }
    }
    private void Start()
    {
        //Sets all rotations to 0
        transform.rotation = Quaternion.Euler(0,transform.rotation.y,0);
    }
    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        m_Camera.Look(m_Input.lookVector);
        m_LookPoint.UpdatePosition(m_Input.lookVector, m_Input.usingKeyboard);
        m_FloatCapsule.Jump(m_Input.jumpTrigger, m_Input.isJumping);
        if(m_Input.interactTrigger)
        {
            m_Interactor.Interact();
        }
    }
    private void FixedUpdate() {    
        m_Movement.Move(m_Input.moveDirection, m_Input.isBoosting);
    }
}
