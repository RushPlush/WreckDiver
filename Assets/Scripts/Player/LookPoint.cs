using UnityEngine;

public class LookPoint : MonoBehaviour
{
    private Transform cameraTrans;
    private float offset;
    [SerializeField] private float sensitivity = 0.1f;
    [SerializeField] private float smoothingSpeed = 4;
    private void Start()
    {
        //cameraTrans = transform.parent;
        offset = transform.localPosition.z;
    }
    public void UpdatePosition(Vector2 position)
    {
        transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(position.x * sensitivity, position.y * sensitivity, offset), Time.deltaTime * smoothingSpeed);
    }
}
