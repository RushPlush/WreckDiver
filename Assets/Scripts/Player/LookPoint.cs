using UnityEngine;

public class LookPoint : MonoBehaviour
{
    private Transform cameraTrans;
    private float offset;    
    [SerializeField] private float keyboardSensitivity = 0.1f;
    [SerializeField] private float mouseSmoothingSpeed = 4;
    private void Start()
    {
        //cameraTrans = transform.parent;
        offset = transform.localPosition.z;
    }
    public void UpdatePosition(Vector2 position, bool isUsingKeyboard)
    {
        if (isUsingKeyboard)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(position.x*keyboardSensitivity, position.y*keyboardSensitivity, offset), Time.deltaTime * mouseSmoothingSpeed);
            //transform.localPosition = new Vector3(position.x*keyboardSensitivity, position.y*keyboardSensitivity, transform.localPosition.z);
        }
        else
        {
            transform.localPosition = new Vector3(position.x, position.y, transform.localPosition.z);
        }
    }
}
