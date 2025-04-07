using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[DisallowMultipleComponent]
public class Lockwheel : MonoBehaviour
{
    [SerializeField, Range(0, 9)] private int codeFace = 0;
    [SerializeField] private int prevCodeFace = 0;
    [SerializeField] private float anglePerFace = -36;
    [SerializeField] private float speedModifier = 1;
    [SerializeField] private float rotationTime = 0;
    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        outline.OutlineColor = Color.HSVToRGB(60 / 360f, 1f, 1f);
        transform.localRotation = GetRotation(codeFace);
    }

    void Update()
    {
        var currentRotation = GetRotation(codeFace);
        if (transform.localRotation != currentRotation)
        {
            var prevRotation = GetRotation(prevCodeFace);
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
        outline.enabled = true;
    }

    public void Unhighlight()
    {
        outline.enabled = false;
    }

    public void SetCodeFace(int face)
    {
        face = Math.Clamp(face, 0, 9);
        codeFace = face;
        prevCodeFace = face;
        transform.localRotation = GetRotation(face);
    }

    private Quaternion GetRotation(int face) => Quaternion.Euler(anglePerFace * face, 0, 0);

    public int GetCodeFace() => prevCodeFace;

    public void Increment() => codeFace++;
    public void Decrement() => codeFace--;
}