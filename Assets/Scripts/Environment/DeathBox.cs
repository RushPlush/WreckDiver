using System;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            var saveAndLoad = other.gameObject.GetComponent<SaveAndLoad>();
            if (saveAndLoad != null)
            {
                saveAndLoad.Death();
            }
        }
    }
}
