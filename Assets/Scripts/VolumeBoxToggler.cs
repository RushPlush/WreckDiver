using UnityEngine;
using UnityEngine.Rendering;

public class VolumeBoxToggler : MonoBehaviour
{
    void Awake()
    {
        var volume = GetComponent<Volume>();
#if UNITY_EDITOR
        volume.enabled = false;
#else
        volume.enabled = true;
#endif
    }
}
