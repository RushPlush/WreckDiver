using UnityEngine;

public class LookPoint : MonoBehaviour
{
    public void UpdatePosition(Vector3 position)
    {
        transform.localPosition = position;
    }
}
