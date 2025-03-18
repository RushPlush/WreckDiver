using System;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    
    private bool isHarpoonActive = false;

    [SerializeField] private float speed = 10f;
    public void Shoot()
    {
        isHarpoonActive = true;
    }

    private void FixedUpdate()
    {
        if (!isHarpoonActive) return;
        transform.Translate(Vector3.forward * speed);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!isHarpoonActive) return;
        transform.parent = other.transform;
        isHarpoonActive = false;
    }
    //todo 
}
