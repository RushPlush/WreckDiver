using UnityEngine;

public class OpenDoor : MonoBehaviour, IUnlockable
{
    [SerializeField] private Vector3 unlockedStatePosition;
    [SerializeField] private Quaternion unlockedStateRotation;
    [SerializeField] private Vector3 unlockedStateScale;
    [SerializeField] private float animationSpeed = 1;

    [SerializeField] private Vector3 lockedStatePosition;
    [SerializeField] private Quaternion lockedStateRotation;
    [SerializeField] private Vector3 lockedStateScale;
    private float animationTime = 0;

    public void Unlock()
    {
        enabled = true;
    }

    void Start()
    {
        lockedStatePosition = gameObject.transform.localPosition;
        lockedStateRotation = gameObject.transform.localRotation;
        lockedStateScale = gameObject.transform.localScale;
        enabled = false;
    }

    void Update()
    {
        animationTime += Time.deltaTime * animationSpeed;
        gameObject.transform.localPosition = Vector3.Lerp(lockedStatePosition, unlockedStatePosition, animationTime);
        gameObject.transform.localRotation = Quaternion.Lerp(lockedStateRotation, unlockedStateRotation, animationTime);
        gameObject.transform.localScale = Vector3.Lerp(lockedStateScale, unlockedStateScale, animationTime);
        if (animationTime > 1) enabled = false;
    }
}