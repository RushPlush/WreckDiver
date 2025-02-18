using UnityEngine;

public class Lockwheel : MonoBehaviour
{
    public float sides = 9;
    public int codeFace = 0;
    public float anglePerFace = -36;
    public float speedModifier = 1;

    void Start()
    {
        transform.localRotation = Quaternion.Euler(0, anglePerFace * codeFace, 90);
    }

    void Update()
    {
        transform.Rotate(anglePerFace * Time.deltaTime * speedModifier, 0, 0);
    }
}
