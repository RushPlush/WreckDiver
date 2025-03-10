using System;
using UnityEngine;

public class Lockwheel : MonoBehaviour
{
    [SerializeField, Range(0, 9)] private int codeFace = 0;
    [SerializeField] private int prevCodeFace = 0;
    [SerializeField] private float anglePerFace = -36;
    [SerializeField] private float speedModifier = 1;
    [SerializeField] private float rotationTime = 0;
    /// <summary>
    /// The material of the lockwheel when it is highlighted
    /// </summary>
    [SerializeField] private Material highlightMaterial;
    /// <summary>
    /// The default material of the lockwheel
    /// </summary>
    [SerializeField] private Material defaultMaterial;

    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.materials[0] = defaultMaterial;
        transform.localRotation = Quaternion.Euler(0, anglePerFace * codeFace, 90);
    }

    void Update()
    {
        var currentRotation = Quaternion.Euler(0, anglePerFace * codeFace, 90);
        if (transform.localRotation != currentRotation)
        {
            var prevRotation = Quaternion.Euler(0, anglePerFace * prevCodeFace, 90);
            float localSpeedModifier = Math.Abs(prevCodeFace - codeFace);
            if (localSpeedModifier > 5)
            {
                localSpeedModifier = 5 + (5 - localSpeedModifier);
            }

            localSpeedModifier *= speedModifier;
            // lerp rotation to the new rotation
            rotationTime += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(prevRotation, currentRotation, rotationTime / localSpeedModifier);
            var distance = transform.localRotation.Distance2(currentRotation);
            // if the rotation is close enough to the new rotation snap to it to avoid jitter
            if (distance < 0.1f)
            {
                transform.localRotation = currentRotation;
                prevCodeFace = codeFace;
                rotationTime = 0;
            }
        }
    }

    public void Highlight()
    {
        renderer.materials[0] = highlightMaterial;
    }

    public void Unhighlight()
    {
        renderer.materials[0] = defaultMaterial;
    }

    public void SetCodeFace(int face)
    {
        face = Math.Clamp(face, 0, 9);
        codeFace = face;
        prevCodeFace = face;
        transform.localRotation = Quaternion.Euler(0, anglePerFace * codeFace, 90);
    }

    public int GetCodeFace() => prevCodeFace;

    public void Increment() => codeFace++;
    public void Decrement() => codeFace--;
}