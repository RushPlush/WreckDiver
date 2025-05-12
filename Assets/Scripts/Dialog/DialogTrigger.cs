using System;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private string dialogText;
    [SerializeField] bool onlyPlayOnce = true;
    private bool hasPlayed = false;
    private DialogSystem dialogSystem;
    [SerializeField] private DialogTrigger[] dependentTriggers;
    [SerializeField][Tooltip("If true, will only play if ANY dependent triggers have been played")]
    private bool onlyPlayIfDependentPlayed = false;
    [SerializeField][Tooltip("If true, will NOT play, only if ALL dependent triggers have been played")] 
    private bool onlyPlayIfDependentNotPlayed = false;
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
                if(dependentTriggers.Length > 0)
                {
                    bool allPlayed = true;
                    bool anyPlayed = false;
                    foreach (var trigger in dependentTriggers)
                    {
                        if (trigger.hasPlayed)
                        {
                            anyPlayed = true;
                        }
                        else
                        {
                            allPlayed = false;
                        }
                    }
                    if (onlyPlayIfDependentPlayed && !anyPlayed)
                    {
                        return;
                    }
                    if (onlyPlayIfDependentNotPlayed && allPlayed)
                    {
                        return;
                    }
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
