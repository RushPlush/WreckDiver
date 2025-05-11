using UnityEngine;

public class Helmet : MonoBehaviour
{
    private Transform playerTrans;
    private Transform cameraTrans;
    private Transform lookPoint;
    public Vector2 Movement;

    [SerializeField] private float deadZoneSize = 20;
    [SerializeField] private float innerDeadZoneSize = 5;
    [SerializeField] private float movementMagnitudeDeadZone = 0.1f;
    [SerializeField] private float springForce = 10;
    [SerializeField][Range(0, 0.3f)] private float dampingTime = 0.1f;
    private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTrans = transform.parent;
        cameraTrans = playerTrans.GetChild(0);
        lookPoint = cameraTrans.GetChild(0);
        offset = transform.localPosition;
        transform.SetParent(null, true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        float angle = Mathf.Abs(Quaternion.Angle(cameraTrans.rotation, transform.rotation));
        transform.localPosition = playerTrans.localPosition + offset * playerTrans.localScale.x;
        if (angle > deadZoneSize || Movement.magnitude > movementMagnitudeDeadZone)
        {
            var curRotation = new Vector4(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            var targetRot = Quaternion.RotateTowards(transform.rotation, cameraTrans.rotation, springForce *
                (angle - innerDeadZoneSize) * Time.deltaTime);
            var targetRotVector = new Vector4(targetRot.x, targetRot.y, targetRot.z, targetRot.w);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, dampingTime); // todo something to bias it in the direction the camera is going, since it's lagging behind were it's trending, do some A B testing to see if it's better

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookPoint.position - transform.position, transform.up), springForce *
            //    (angle - innerDeadZoneSize) * Time.deltaTime);
        }
        else if (angle < innerDeadZoneSize)
        {

        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, cameraTrans.rotation, springForce *
                (angle - innerDeadZoneSize) / 4 * Time.deltaTime);
        }

        
    }
}
