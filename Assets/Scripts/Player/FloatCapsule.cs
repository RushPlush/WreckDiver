using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class FloatCapsule : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("RayCast")]
    [SerializeField] private float rayLenght = 1f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Levitating")][Tooltip("How the capsule interacts with the ground, having it be levitating makes ledges and slopes easier to traverse.")]
    [SerializeField] private float rideHeight = 0.1f;
    [SerializeField] private float rideSpringDampener = 10f;
    [SerializeField] private float rideSpringStrength = 5f;

    [Header("Gravity")]
    [SerializeField] private float extraGravity = 20f;

    private float springForce;
    
    [Header("Jump")]
    private bool buffer = false;
    [SerializeField][Tooltip("The amount of time wherein it will execute a jump when it becomes possible")] 
    private float bufferTime = 0.2f;
    private float bufferTimer;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField][Tooltip("The amount of force you get per frame when holding down the jump button. only works whilst the player has a positive y velocity")] 
    private float sustainJumpForce = 3f;
    [SerializeField][Tooltip("The amount of time you can sustain a jump. To avoid large force values making the character effectively fly")]
    private float sustainJumpTime  = .3f;
    private float sustainJumpTimer = 0;
    private bool  canSustain;

    [SerializeField][Tooltip("The delay wherein the player can't jump again. This prevents the player from being able to get multiple jumps in a row before leaving the grounded state. (Jumping higher than they're supposed to)")]
    private float jumpDelay = 0.3f;
    private float jumpTimer;
    [FormerlySerializedAs("jumpAble")] public bool canJump = true;
    
    [FormerlySerializedAs("grounded")] [Header("Ground Check")]
    public bool isGrounded = true;

    [Header("Coyote Time")][Tooltip("Coyote Time refers to the ability to jump after falling over a ledge, this helps make timing jumps more forgiving")]
    public bool coyoteTime = true;
    [SerializeField] private float coyoteDelay = 0.2f;
    private  float coyoteTimer;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rayLenght = rayLenght + 1;
        rideHeight = rideHeight + 1;
    }
    private void Update()
    {
        CoyoteTime();
    }
    private void FixedUpdate()
    {
        GroundCast();
        FakeGravity();
    }
    private void GroundCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, rayLenght, groundLayer))
        {
            float groundedDistanceLeniency = -0.45f;
            if (hit.distance <= rideHeight + groundedDistanceLeniency)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            Vector3 vel = rb.linearVelocity;
            Vector3 rayDir = transform.TransformDirection(-transform.up);

            float rayDirVel = Vector3.Dot(rayDir, vel);
            if (!isGrounded) return;

            float x = hit.distance - rideHeight;

            springForce = (x * rideSpringStrength) - (rayDirVel * rideSpringDampener);

            if (!canJump)
            {
                springForce = 0; // Here to avoid the spring from stopping the jump. resulting in inconsistent jumps.
            }
            rb.AddForce(rayDir * springForce);
        }
        else
        {
            isGrounded = false;
        }
    }
    private void FakeGravity()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);
        }
    }
    private void CoyoteTime()
    {
        if (isGrounded)
        {
            //doubleJumpAble = false;
            coyoteTime = true;
            coyoteTimer = Time.timeSinceLevelLoad + coyoteDelay;
        }
        else if (Time.timeSinceLevelLoad >= coyoteTimer)
        {
            coyoteTime = false;
        }
    }
    public void Jump(bool jumpTriggered, bool jumpValue) // todo refactor into try jump and do jump
    {
        if (jumpTriggered && coyoteTime && canJump || buffer && coyoteTime && canJump)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            coyoteTime = false;
            canJump = false;
            canSustain = true;
            sustainJumpTimer = 0;
            jumpTimer = Time.timeSinceLevelLoad + jumpDelay;
        }
        else if(jumpTriggered)
        {
            buffer = true;
            bufferTimer = bufferTime + Time.timeSinceLevelLoad;
        }
        if (buffer)
        {
            if (Time.timeSinceLevelLoad >= bufferTimer)
            {
                buffer = false;
            }
        }
        if (canSustain)
        {
            sustainJumpTimer += Time.deltaTime;
            canSustain = jumpValue; // catches if the user lets go of the jump key
            
        }
        if (Time.timeSinceLevelLoad >= jumpTimer)
        {
            canJump = true;
        }

        // todo add a max amount of time that force can be added, otherwise it makes a very flat topped jump. 
        if (jumpValue && rb.linearVelocity.y > 0 && canSustain && sustainJumpTimer <= sustainJumpTime)
        {
            rb.AddForce(transform.up * sustainJumpForce, ForceMode.Force);
        }
    }
}
