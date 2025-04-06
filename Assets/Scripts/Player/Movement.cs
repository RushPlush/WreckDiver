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
    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rb.freezeRotation = true;
        cam.fieldOfView = fov;
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
}
