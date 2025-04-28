using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float moveSpeed = 5f;
    private float movementMultiplier = 10f;

    [SerializeField] private float runMultiplier = 2f;

    private Rigidbody rb;
    private float drag = 5.5f;
    private Vector3 moveDirection;

    [SerializeField] private float fov = 60;
    private float runFOV = 10;

    private MappingHandler mappingHandler;
    private float lastMoveTime;
    [SerializeField] private float hintDelay = 1;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        mappingHandler = GetComponentInChildren<MappingHandler>();
    }

    private void Start()
    {
        rb.freezeRotation = true;
        cam.fieldOfView = fov;
    }

    private void Update()
    {
        if (Time.time - lastMoveTime > hintDelay && !mappingHandler.IsMoveGuideActive())
        {
            mappingHandler.ToggleMoveGuide();
        }
    }

    private void FixedUpdate()
    {
        Drag();
    }

    void Drag()
    {
        Vector3 velocity = transform.InverseTransformDirection(rb.linearVelocity);
        float forceX = -drag * velocity.x;
        float forceZ = -drag * velocity.z;

        rb.AddRelativeForce(new Vector3(forceX, 0, forceZ));
    }

    public void Move(Vector2 moveVector, bool run)
    {
        if(moveVector != Vector2.zero)
            lastMoveTime = Time.time;
        if (mappingHandler.IsMoveGuideActive() && lastMoveTime == Time.time) mappingHandler.ToggleMoveGuide();
        moveDirection = transform.forward * moveVector.y + transform.right * moveVector.x;
        if (run)
        {
            rb.AddForce(moveDirection.normalized * (moveSpeed * movementMultiplier * runMultiplier), ForceMode.Acceleration);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov + runFOV, 0.1f);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * (moveSpeed * movementMultiplier), ForceMode.Acceleration);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, 0.1f);
        }
    }

    public void ResetMoveTimer()
    {
        lastMoveTime = Time.time;
        if (mappingHandler.IsMoveGuideActive()) mappingHandler.ToggleMoveGuide();
    }

    public void StopMoving()
    {
        rb.isKinematic = true;
    }

    public void StartMoving()
    {
        rb.isKinematic = false;
    }
}
