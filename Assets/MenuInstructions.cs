using System;
using UnityEngine;

public class MenuInstructions : MonoBehaviour
{
    public bool StartOnCreditScreen = false;
    public bool PlayMenuAnimaticOnAwake = true;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        var menuIns = Resources.FindObjectsOfTypeAll<MenuInstructions>();
        foreach (var menuInstruction in menuIns)
        {
            if(menuInstruction != this && menuInstruction != null)
            {
                Destroy(this.gameObject); // Ensure only one instance exists, keeping the oldest one. 
                return;
            }
        }
    }
}
