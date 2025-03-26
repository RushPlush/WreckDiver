using System;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    
    private bool isHarpoonActive = false;
    [SerializeField] private LayerMask afterImpactLayer;
    [SerializeField] private float speed = 10f;
    Rigidbody rb;

    private void Awake()
    {
        rb=GetComponent<Rigidbody>();
    }

    public void Shoot()
    {
        isHarpoonActive = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(-transform.up * speed, ForceMode.Impulse);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        if (!isHarpoonActive) return;
        //transform.Translate(Vector3.forward * speed);
    }

    int GetLayerNumber(LayerMask layerMaskIn)
    {
        int layerNumber = -1;
        int layer = layerMaskIn.value;
        while(layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber;
    }
    private void OnCollisionEnter(Collision other)
    {
        if(!isHarpoonActive) return;
        transform.parent = other.transform;
        isHarpoonActive = false;
        print(GetLayerNumber(afterImpactLayer));
        gameObject.layer = GetLayerNumber(afterImpactLayer);
        print(gameObject.layer);
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.interpolation = RigidbodyInterpolation.None;
    }
    //todo 
}
