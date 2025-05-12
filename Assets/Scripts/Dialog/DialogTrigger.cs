using System;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private string dialogText;
    [SerializeField] bool onlyPlayOnce = true;
    [SerializeField][Tooltip("If true, the dialog box will not open, but this trigger will register as played. Useful as dependency triggers")] private bool dontOpenDialogBox = false;
    private bool hasPlayed = false;
    private DialogSystem dialogSystem;
    [SerializeField] private DialogTrigger[] dependentTriggers;
    [SerializeField][Tooltip("If true, will only play if ANY dependent triggers have been played")]
    private bool onlyPlayIfDependentPlayed = true;
    [SerializeField][Tooltip("If true, will only play if ALL dependent triggers have been played")]
    private bool onlyPlayIfAllDependentPlayed = false;
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
                    if(onlyPlayIfAllDependentPlayed && !allPlayed)
                    {
                        return;
                    }
                }
                hasPlayed = true;
                if (dontOpenDialogBox)
                {
                    return;
                }
                dialogSystem.SetDialog(dialogText);
                

            }
            else
            {
                Debug.LogWarning("DialogSystem not found in the scene.");
            }
        }
    }
}
