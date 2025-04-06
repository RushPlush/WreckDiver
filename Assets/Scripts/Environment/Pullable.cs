using System;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pullable : MonoBehaviour
{
    private Rigidbody rb;
    private bool DefaultIsKinematic;
    private CollisionDetectionMode DefaultCollisionMode;
    private RigidbodyInterpolation DefaultInterpolation;
    
    private bool      isPulled = false;
    private bool      hasOtherPullable = false;
    private Pullable  otherPullable; // for cases when two pullables are connected
    private Transform puller;
    [SerializeField] private float pullForce = 10f;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        DefaultIsKinematic = rb.isKinematic;
        DefaultCollisionMode = rb.collisionDetectionMode;
        DefaultInterpolation = rb.interpolation;
    }

    public void Pull()
    {
        isPulled = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    public void Pull(Pullable other)
    {
        otherPullable = other;
        puller = otherPullable.transform;
        if(otherPullable.otherPullable == null)
            otherPullable.Pull(this);
        Pull();
    }
    public void Pull(Transform puller)
    {
        this.puller = puller;
        Pull();
    }

    public void Stop()
    {
        isPulled = false;
    }
    
    public void Release()
    {
        isPulled = false;
        if(otherPullable != null)
        {
            otherPullable.Release();
            otherPullable = null;
        }
        puller = null; 
        rb.isKinematic = DefaultIsKinematic;
        rb.collisionDetectionMode = DefaultCollisionMode;
        rb.interpolation = DefaultInterpolation;
    }
    private void FixedUpdate()
    {
        if(!isPulled) return;
        float distance = Vector3.Distance(transform.position, puller.position);
        // distance multiplier to make the pull force stronger the further away the puller is
        float distanceMultiplier = distance / 10;
        Vector3 direction = Vector3.Normalize(puller.position - transform.position);
        rb.AddForce(pullForce * distanceMultiplier * direction, ForceMode.Force);
    }
}

