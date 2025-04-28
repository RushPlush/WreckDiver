using System;
using UnityEngine;

public class NoGrappleZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Harpoon"))
            return;
        Harpoon harpoon = other.GetComponent<Harpoon>();
        if (harpoon == null)
            return;
        harpoon.SetCanPull(false);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Harpoon"))
        {
            
        }
    }
}
