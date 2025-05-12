using System;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private string dialogText;
    [SerializeField] bool onlyPlayOnce = true;
    private bool hasPlayed = false;
    private DialogSystem dialogSystem;
    private void Awake()
    {
        dialogSystem = FindFirstObjectByType<DialogSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (dialogSystem != null)
            {
                if(hasPlayed && onlyPlayOnce)
                {
                    return;
                }
                dialogSystem.SetDialog(dialogText);
                hasPlayed = true;

            }
            else
            {
                Debug.LogWarning("DialogSystem not found in the scene.");
            }
        }
    }
}
